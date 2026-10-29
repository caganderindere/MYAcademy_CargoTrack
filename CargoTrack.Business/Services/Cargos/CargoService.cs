using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Helpers;
using CargoTrack.Business.Rules;
using CargoTrack.Business.Services.CargoPricing;
using CargoTrack.Business.Services.CurrentUser;
using CargoTrack.DataAccess.Repositories.Addresses;
using CargoTrack.DataAccess.Repositories.Cargo;
using CargoTrack.DataAccess.Repositories.CargoMovements;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.DTO.DTOs.Common;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace CargoTrack.Business.Services.Cargos
{
    public class CargoService(ICargoRepository _cargoRepository,
                              ICargoMovementRepository _cargoMovementRepository,
                              IAddressRepository _addressRepository,
                              IEmployeeRepository _employeeRepository,
                              ICargoPricingService _cargoPricingService,
                              IUserContextService _userContextService) : ICargoService
    {
        private const int MaxFailedDeliveryAttempts = 3;
        private const string UnauthorizedMessage = "Bu işlem için yetkiniz yok.";
        private const string NotFoundMessage = "Kargo bulunamadı.";
        private const string IncomingDirection = "incoming";
        private const string OutgoingDirection = "outgoing";

        private static readonly CargoStatus[] StatusesWithOwnEndpoint =
        {
            CargoStatus.Delivered,
            CargoStatus.DeliveryFailed
        };

        public async Task<PagedResultDto<ResultCargoDto>> GetPagedAsync(CargoFilterDto filter)
        {
            var (page, pageSize) = filter.Normalize();
            var now = DateTime.Now;

            var query = await GetScopeAsync(filter.Direction);

            var search = string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search.Trim();
            var status = filter.Status;
            var branchId = filter.BranchId;
            var dateFrom = filter.DateFrom?.Date;
            var dateTo = filter.DateTo?.Date.AddDays(1);

            query = query.And(x =>
                (search == null
                    || x.TrackCode.Contains(search)
                    || x.Sender.FirstName.Contains(search)
                    || x.Sender.LastName.Contains(search)
                    || x.Receiver.FirstName.Contains(search)
                    || x.Receiver.LastName.Contains(search))
                && (status == null || x.CargoStatus == status)
                && (branchId == null || x.OriginBranchId == branchId || x.DestinationBranchId == branchId)
                && (dateFrom == null || x.CreatedDate >= dateFrom)
                && (dateTo == null || x.CreatedDate < dateTo));

            if (filter.OnlyDelayed)
            {
                query = query.And(CargoSlaRules.DelayedExpression(now));
            }

            if (filter.OnlyActive)
            {
                query = query.And(CargoSlaRules.ActiveExpression());
            }

            if (filter.OnlyFinished)
            {
                query = query.And(x => CargoSlaRules.FinishedStatuses.Contains(x.CargoStatus));
            }

            if (filter.OnlyProblematic)
            {
                query = query.And(x => CargoSlaRules.ProblematicStatuses.Contains(x.CargoStatus));
            }

            var paged = await _cargoRepository.GetPagedAsync(page, pageSize, query);
            var result = paged.ToPagedResult<Cargo, ResultCargoDto>();

            foreach (var item in result.Items)
            {
                item.IsDelayed = CargoSlaRules.IsDelayed(item.CargoStatus, item.EstimatedDeliveryDate, item.DeliveredDate, now);
            }

            return result;
        }

        public async Task<CargoDetailDto> GetDetailAsync(Guid id)
        {
            var cargo = await _cargoRepository.GetByIdAsync(id) ?? throw new BusinessException(NotFoundMessage);

            var canManage = await CanManageAsync(cargo);
            var customer = _userContextService.IsCustomer && !_userContextService.IsAdmin && !_userContextService.IsManager
                ? await _userContextService.GetCurrentCustomerAsync()
                : null;
            var isOwner = customer is not null && (cargo.SenderId == customer.Id || cargo.ReceiverId == customer.Id);

            if (!canManage && !isOwner)
            {
                throw new BusinessException("Bu kargonun detaylarını görüntüleme yetkiniz yok.");
            }

            var detail = cargo.Adapt<CargoDetailDto>();

            detail.Movements = cargo.Movements
                .OrderByDescending(x => x.MovementDate)
                .Adapt<List<ResultCargoMovementDto>>();
            detail.DeliveryExceptions = cargo.DeliveryExceptions
                .OrderByDescending(x => x.OccurredDate)
                .Adapt<List<DeliveryExceptionDto>>();
            detail.IsDelayed = CargoSlaRules.IsDelayed(cargo.CargoStatus, cargo.EstimatedDeliveryDate, cargo.DeliveredDate, DateTime.Now);
            detail.CanManage = canManage;
            detail.IsReceiver = customer is not null && cargo.ReceiverId == customer.Id;
            detail.DeliveryCode = detail.IsReceiver && cargo.CargoStatus == CargoStatus.OutForDelivery ? cargo.DeliveryCode : null;

            if (canManage)
            {
                detail.NextStatuses = CargoStatusRules.GetNextStatuses(cargo.CargoStatus)
                    .Where(x => !StatusesWithOwnEndpoint.Contains(x))
                    .ToList();

                if (_userContextService.IsManager)
                {
                    detail.CurrentEmployeeId = (await _userContextService.GetCurrentEmployeeAsync()).Id;
                }
                else
                {
                    var employees = await _employeeRepository.GetListAsync(
                        x => x.BranchId == cargo.DestinationBranchId,
                        q => q.OrderBy(x => x.FirstName));

                    detail.AssignableEmployees = employees
                        .Select(x => new OptionDto { Id = x.Id, Name = $"{x.FirstName} {x.LastName}" })
                        .ToList();
                }
            }

            return detail;
        }

        public async Task<PublicTrackingDto> GetPublicTrackingAsync(string trackCode)
        {
            if (string.IsNullOrWhiteSpace(trackCode))
            {
                throw new BusinessException("Lütfen bir takip numarası girin.");
            }

            var cargo = await _cargoRepository.GetByTrackCodeAsync(trackCode.Trim().ToUpperInvariant())
                ?? throw new BusinessException("Bu takip numarasına ait bir gönderi bulunamadı.");

            var movements = cargo.Movements
                .OrderByDescending(x => x.MovementDate)
                .Select(x => new PublicMovementDto
                {
                    ToStatus = x.ToStatus,
                    MovementDate = x.MovementDate,
                    LocationName = x.Branch?.Name ?? x.TransferCenter?.Name,
                    Description = x.ToStatus == CargoStatus.Delivered ? "Gönderi teslim edildi." : x.Description
                })
                .ToList();

            return new PublicTrackingDto
            {
                TrackCode = cargo.TrackCode,
                CargoStatus = cargo.CargoStatus,
                CargoType = cargo.CargoType,
                DeliveryType = cargo.DeliveryType,
                SenderMaskedName = PrivacyMasker.MaskFullName(cargo.Sender.FirstName, cargo.Sender.LastName),
                ReceiverMaskedName = PrivacyMasker.MaskFullName(cargo.Receiver.FirstName, cargo.Receiver.LastName),
                OriginCityName = cargo.OriginBranch.City.Name,
                OriginBranchName = cargo.OriginBranch.Name,
                DestinationCityName = cargo.DestinationBranch.City.Name,
                DestinationBranchName = cargo.DestinationBranch.Name,
                CreatedDate = cargo.CreatedDate,
                EstimatedDeliveryDate = cargo.EstimatedDeliveryDate,
                DeliveredDate = cargo.DeliveredDate,
                IsDelayed = CargoSlaRules.IsDelayed(cargo.CargoStatus, cargo.EstimatedDeliveryDate, cargo.DeliveredDate, DateTime.Now),
                FailedDeliveryAttempts = cargo.FailedDeliveryAttempts,
                LastMovement = movements.FirstOrDefault(),
                Movements = movements
            };
        }

        public async Task<PagedResultDto<ResultCargoMovementDto>> GetMovementsAsync(MovementFilterDto filter)
        {
            var (page, pageSize) = filter.Normalize();
            var day = (filter.Date ?? DateTime.Today).Date;
            var nextDay = day.AddDays(1);
            var search = string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search.Trim();

            Expression<Func<CargoMovement, bool>> query = x => x.MovementDate >= day && x.MovementDate < nextDay
                && (search == null || x.Cargo.TrackCode.Contains(search));

            if (!_userContextService.IsAdmin)
            {
                if (!_userContextService.IsManager)
                {
                    throw new BusinessException(UnauthorizedMessage);
                }

                var employee = await _userContextService.GetCurrentEmployeeAsync();

                if (employee.BranchId is not null)
                {
                    var branchId = employee.BranchId.Value;
                    query = query.And(x => x.BranchId == branchId
                                        || x.Cargo.OriginBranchId == branchId
                                        || x.Cargo.DestinationBranchId == branchId);
                }
                else
                {
                    var transferCenterId = employee.TransferCenterId;
                    query = query.And(x => x.TransferCenterId == transferCenterId);
                }
            }

            var paged = await _cargoMovementRepository.GetPagedAsync(page, pageSize, query, q => q.OrderByDescending(x => x.MovementDate));
            return paged.ToPagedResult<CargoMovement, ResultCargoMovementDto>();
        }

        public async Task<CargoCreateContextDto> GetCreateContextAsync()
        {
            if (_userContextService.IsAdmin)
            {
                return new CargoCreateContextDto();
            }

            if (!_userContextService.IsManager)
            {
                throw new BusinessException(UnauthorizedMessage);
            }

            var employee = await _userContextService.GetCurrentEmployeeAsync();

            if (employee.BranchId is null)
            {
                throw new BusinessException("Kargo kabulü yalnızca şube personeli tarafından yapılabilir.");
            }

            return new CargoCreateContextDto
            {
                FixedOriginBranchId = employee.BranchId,
                FixedOriginBranchName = employee.Branch?.Name
            };
        }

        public async Task<CargoQuoteDto> QuoteAsync(CreateCargoDto createCargoDto)
        {
            await EnsureCanCreateAsync(createCargoDto.OriginBranchId);

            var (desi, price) = await _cargoPricingService.CalculateAsync(createCargoDto);

            return new CargoQuoteDto
            {
                Desi = desi,
                Price = price,
                EstimatedDeliveryDate = DeliveryDateRules.CalculateEstimatedDeliveryDate(DateTime.Now, createCargoDto.DeliveryType, createCargoDto.AdditionalServices)
            };
        }

        public async Task<string> CreateAsync(CreateCargoDto createCargoDto)
        {
            await EnsureCanCreateAsync(createCargoDto.OriginBranchId);

            var deliveryAddress = await _addressRepository.GetByIdAsync(createCargoDto.DeliveryAddressId);

            if (deliveryAddress is null || deliveryAddress.CustomerId != createCargoDto.ReceiverId)
            {
                throw new BusinessException("Teslimat adresi seçilen alıcıya ait olmalıdır.");
            }

            var (desi, price) = await _cargoPricingService.CalculateAsync(createCargoDto);

            var cargo = createCargoDto.Adapt<Cargo>();
            var now = DateTime.Now;

            cargo.Desi = desi;
            cargo.Price = price;
            cargo.EstimatedDeliveryDate = DeliveryDateRules.CalculateEstimatedDeliveryDate(now, cargo.DeliveryType, cargo.AdditionalServices);
            cargo.TrackCode = await GenerateTrackCodeAsync();
            cargo.CargoStatus = CargoStatus.Created;

            cargo.Movements = new List<CargoMovement>
            {
                new CargoMovement
                {
                    FromStatus = null,
                    ToStatus = CargoStatus.Created,
                    MovementDate = now,
                    BranchId = cargo.OriginBranchId,
                    EmployeeId = await GetCurrentEmployeeIdAsync(),
                    Description = "Gönderi oluşturuldu."
                }
            };

            await _cargoRepository.CreateAsync(cargo);

            return cargo.TrackCode;
        }

        public async Task ChangeStatusAsync(ChangeCargoStatusDto changeCargoStatusDto)
        {
            var cargo = await GetManageableCargoAsync(changeCargoStatusDto.CargoId);
            var employeeId = await GetCurrentEmployeeIdAsync() ?? changeCargoStatusDto.EmployeeId;

            var oldStatus = cargo.CargoStatus;
            var newStatus = changeCargoStatusDto.NewStatus;

            if (!CargoStatusRules.CanTransition(oldStatus, newStatus))
            {
                throw new BusinessException($"Kargo '{oldStatus}' durumundan '{newStatus}' durumuna geçemez.");
            }

            if (newStatus == CargoStatus.Delivered)
            {
                throw new BusinessException("Teslim işlemi, teslimat kodu doğrulanarak yapılmalıdır.");
            }

            if (newStatus == CargoStatus.DeliveryFailed)
            {
                throw new BusinessException("Teslimat sorunu, nedeni belirtilerek bildirilmelidir.");
            }

            cargo.CargoStatus = newStatus;

            if (newStatus == CargoStatus.OutForDelivery)
            {
                cargo.DeliveryCode = GenerateDeliveryCode();
                cargo.DeliveryCodeGeneratedDate = DateTime.Now;
            }

            var movement = new CargoMovement
            {
                CargoId = cargo.Id,
                FromStatus = oldStatus,
                ToStatus = newStatus,
                MovementDate = DateTime.Now,
                Description = changeCargoStatusDto.Description,
                BranchId = changeCargoStatusDto.BranchId,
                TransferCenterId = changeCargoStatusDto.TransferCenterId,
                EmployeeId = employeeId
            };

            await _cargoRepository.AddMovementAsync(movement);
        }

        public async Task DeliverAsync(DeliverCargoDto deliverCargoDto)
        {
            var cargo = await GetManageableCargoAsync(deliverCargoDto.CargoId);
            var employeeId = await GetCurrentEmployeeIdAsync() ?? deliverCargoDto.EmployeeId;

            if (cargo.CargoStatus != CargoStatus.OutForDelivery)
            {
                throw new BusinessException("Sadece dağıtımdaki kargolar teslim edilebilir.");
            }

            if (cargo.DeliveryCode != deliverCargoDto.DeliveryCode)
            {
                throw new BusinessException("Teslimat kodu hatalı.");
            }

            var now = DateTime.Now;

            cargo.CargoStatus = CargoStatus.Delivered;
            cargo.DeliveredDate = now;
            cargo.DeliveryCode = null;

            var delivery = new Delivery
            {
                CargoId = cargo.Id,
                EmployeeId = employeeId,
                ReceivedByName = deliverCargoDto.ReceivedByName,
                DeliveredDate = now,
                Note = deliverCargoDto.Note
            };

            var movement = new CargoMovement
            {
                CargoId = cargo.Id,
                FromStatus = CargoStatus.OutForDelivery,
                ToStatus = CargoStatus.Delivered,
                MovementDate = now,
                Description = $"Teslim edildi. Teslim alan: {deliverCargoDto.ReceivedByName}",
                BranchId = cargo.DestinationBranchId,
                EmployeeId = employeeId
            };

            await _cargoRepository.AddDeliveryAsync(delivery, movement);
        }

        public async Task ReportFailedDeliveryAsync(FailedDeliveryDto failedDeliveryDto)
        {
            var cargo = await GetManageableCargoAsync(failedDeliveryDto.CargoId);
            var employeeId = await GetCurrentEmployeeIdAsync() ?? failedDeliveryDto.EmployeeId;

            if (cargo.CargoStatus != CargoStatus.OutForDelivery)
            {
                throw new BusinessException("Sadece dağıtımdaki kargolar için teslimat sorunu bildirilebilir.");
            }

            var now = DateTime.Now;

            cargo.FailedDeliveryAttempts++;
            cargo.CargoStatus = CargoStatus.DeliveryFailed;
            cargo.DeliveryCode = null;

            var deliveryException = new DeliveryException
            {
                CargoId = cargo.Id,
                EmployeeId = employeeId,
                Reason = failedDeliveryDto.Reason,
                Description = failedDeliveryDto.Description,
                AttemptNumber = cargo.FailedDeliveryAttempts,
                OccurredDate = now
            };

            var movements = new List<CargoMovement>
            {
                new CargoMovement
                {
                    CargoId = cargo.Id,
                    FromStatus = CargoStatus.OutForDelivery,
                    ToStatus = CargoStatus.DeliveryFailed,
                    MovementDate = now,
                    Description = $"Teslim edilemedi ({cargo.FailedDeliveryAttempts}. deneme).",
                    BranchId = cargo.DestinationBranchId,
                    EmployeeId = employeeId
                }
            };

            if (cargo.FailedDeliveryAttempts >= MaxFailedDeliveryAttempts)
            {
                cargo.CargoStatus = CargoStatus.ReturnInProgress;

                movements.Add(new CargoMovement
                {
                    CargoId = cargo.Id,
                    FromStatus = CargoStatus.DeliveryFailed,
                    ToStatus = CargoStatus.ReturnInProgress,
                    MovementDate = now.AddSeconds(1),
                    Description = $"{MaxFailedDeliveryAttempts} başarısız teslimat denemesi sonrası iade süreci başlatıldı.",
                    BranchId = cargo.DestinationBranchId
                });
            }

            await _cargoRepository.AddDeliveryExceptionAsync(deliveryException, movements);
        }

        private async Task<Expression<Func<Cargo, bool>>> GetScopeAsync(string? direction)
        {
            var incoming = direction == IncomingDirection;
            var outgoing = direction == OutgoingDirection;

            if (_userContextService.IsAdmin)
            {
                return x => true;
            }

            if (_userContextService.IsManager)
            {
                var employee = await _userContextService.GetCurrentEmployeeAsync();

                if (employee.BranchId is not null)
                {
                    var branchId = employee.BranchId.Value;

                    if (incoming)
                    {
                        return x => x.DestinationBranchId == branchId;
                    }

                    if (outgoing)
                    {
                        return x => x.OriginBranchId == branchId;
                    }

                    return x => x.OriginBranchId == branchId || x.DestinationBranchId == branchId;
                }

                var transferCenterId = employee.TransferCenterId;

                return x => x.OriginBranch.TransferCenterId == transferCenterId
                         || x.DestinationBranch.TransferCenterId == transferCenterId;
            }

            var customer = await _userContextService.GetCurrentCustomerAsync();
            var customerId = customer.Id;

            if (incoming)
            {
                return x => x.ReceiverId == customerId;
            }

            if (outgoing)
            {
                return x => x.SenderId == customerId;
            }

            return x => x.SenderId == customerId || x.ReceiverId == customerId;
        }

        private async Task<bool> CanManageAsync(Cargo cargo)
        {
            if (_userContextService.IsAdmin)
            {
                return true;
            }

            if (!_userContextService.IsManager)
            {
                return false;
            }

            var employee = await _userContextService.GetCurrentEmployeeAsync();

            return employee.BranchId is not null
                ? cargo.OriginBranchId == employee.BranchId || cargo.DestinationBranchId == employee.BranchId
                : cargo.OriginBranch.TransferCenterId == employee.TransferCenterId || cargo.DestinationBranch.TransferCenterId == employee.TransferCenterId;
        }

        private async Task EnsureCanCreateAsync(Guid originBranchId)
        {
            if (_userContextService.IsAdmin)
            {
                return;
            }

            if (!_userContextService.IsManager)
            {
                throw new BusinessException(UnauthorizedMessage);
            }

            var employee = await _userContextService.GetCurrentEmployeeAsync();

            if (employee.BranchId != originBranchId)
            {
                throw new BusinessException("Sadece kendi şubenizden kargo gönderisi oluşturabilirsiniz.");
            }
        }

        private async Task<Cargo> GetManageableCargoAsync(Guid cargoId)
        {
            var cargo = await _cargoRepository.GetByIdAsync(cargoId) ?? throw new BusinessException(NotFoundMessage);

            if (!_userContextService.IsAdmin && !_userContextService.IsManager)
            {
                throw new BusinessException(UnauthorizedMessage);
            }

            if (!await CanManageAsync(cargo))
            {
                throw new BusinessException("Bu kargo sizin biriminizle ilgili değil. İşlem yapamazsınız.");
            }

            return cargo;
        }

        private async Task<Guid?> GetCurrentEmployeeIdAsync()
        {
            if (!_userContextService.IsManager)
            {
                return null;
            }

            var employee = await _userContextService.GetCurrentEmployeeAsync();
            return employee.Id;
        }

        private async Task<string> GenerateTrackCodeAsync()
        {
            string trackCode;

            do
            {
                trackCode = $"MYC-{DateTime.Now.Year}-{Random.Shared.Next(100000, 1000000)}";
            }
            while (await _cargoRepository.GetByTrackCodeAsync(trackCode) is not null);

            return trackCode;
        }

        private static string GenerateDeliveryCode()
        {
            return RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
        }
    }
}
