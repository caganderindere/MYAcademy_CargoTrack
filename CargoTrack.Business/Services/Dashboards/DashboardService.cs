using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Helpers;
using CargoTrack.Business.Rules;
using CargoTrack.Business.Services.Cargos;
using CargoTrack.Business.Services.CurrentUser;
using CargoTrack.DataAccess.Repositories.Branches;
using CargoTrack.DataAccess.Repositories.Cargo;
using CargoTrack.DataAccess.Repositories.CargoMovements;
using CargoTrack.DataAccess.Repositories.Cities;
using CargoTrack.DataAccess.Repositories.Customers;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.DataAccess.Repositories.Reports;
using CargoTrack.DataAccess.Repositories.TransferCenters;
using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.DTO.DTOs.Common;
using CargoTrack.DTO.DTOs.DashboardDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;
using System.Linq.Expressions;

namespace CargoTrack.Business.Services.Dashboards
{
    public class DashboardService(ICargoRepository _cargoRepository,
                                  ICargoMovementRepository _cargoMovementRepository,
                                  IBranchRepository _branchRepository,
                                  IEmployeeRepository _employeeRepository,
                                  ICustomerRepository _customerRepository,
                                  ICityRepository _cityRepository,
                                  ITransferCenterRepository _transferCenterRepository,
                                  IReportRepository _reportRepository,
                                  IUserContextService _userContextService,
                                  ICargoService _cargoService) : IDashboardService
    {
        private const int TopBranchPeriodDays = 30;

        public async Task<AdminSummaryDto> GetAdminSummaryAsync()
        {
            EnsureAdmin();

            var now = DateTime.Now;
            var total = await _cargoRepository.CountAsync();
            var delivered = await _cargoRepository.CountAsync(x => x.CargoStatus == CargoStatus.Delivered);
            var returned = await _cargoRepository.CountAsync(x => x.CargoStatus == CargoStatus.ReturnedToSender);
            var finished = delivered + returned;

            return new AdminSummaryDto
            {
                TotalCargos = total,
                ActiveCargos = await _cargoRepository.CountAsync(CargoSlaRules.ActiveExpression()),
                DelayedCargos = await _cargoRepository.CountAsync(CargoSlaRules.DelayedExpression(now)),
                DeliveredCargos = delivered,
                DeliveryRate = finished == 0 ? 0 : Math.Round(delivered * 100.0 / finished, 1),
                ActiveBranches = await _branchRepository.CountAsync(x => x.IsActive),
                ActiveEmployees = await _employeeRepository.CountAsync(),
                TotalCustomers = await _customerRepository.CountAsync(),
                TodayCargos = await _cargoRepository.CountAsync(x => x.CreatedDate >= now.Date)
            };
        }

        public async Task<ManagerSummaryDto> GetManagerSummaryAsync()
        {
            if (!_userContextService.IsManager)
            {
                throw new BusinessException("Bu panel yalnızca şube ve transfer merkezi personeli içindir.");
            }

            var employee = await _userContextService.GetCurrentEmployeeAsync();
            var now = DateTime.Now;
            var today = now.Date;
            var scope = await GetCargoScopeAsync();

            var summary = new ManagerSummaryDto
            {
                UnitName = employee.Branch?.Name ?? employee.TransferCenter?.Name ?? "-",
                EmployeeFullName = $"{employee.FirstName} {employee.LastName}",
                DelayedCargos = await _cargoRepository.CountAsync(scope.And(CargoSlaRules.DelayedExpression(now))),
                ProblematicCargos = await _cargoRepository.CountAsync(scope.And(x => CargoSlaRules.ProblematicStatuses.Contains(x.CargoStatus))),
                ActiveCargos = await _cargoRepository.CountAsync(scope.And(CargoSlaRules.ActiveExpression()))
            };

            if (employee.BranchId is not null)
            {
                var branchId = employee.BranchId.Value;

                summary.TodayCreated = await _cargoRepository.CountAsync(x => x.OriginBranchId == branchId && x.CreatedDate >= today);
                summary.TodayIncoming = await _cargoMovementRepository.CountAsync(x => x.BranchId == branchId && x.ToStatus == CargoStatus.AtDestinationBranch && x.MovementDate >= today);
                summary.TodayOutgoing = await _cargoMovementRepository.CountAsync(x => x.Cargo.OriginBranchId == branchId && x.FromStatus == CargoStatus.AtOriginBranch && x.MovementDate >= today);
                summary.OutForDelivery = await _cargoRepository.CountAsync(x => x.DestinationBranchId == branchId && x.CargoStatus == CargoStatus.OutForDelivery);
                summary.DeliveredToday = await _cargoRepository.CountAsync(x => x.DestinationBranchId == branchId && x.DeliveredDate >= today);
            }
            else
            {
                var transferCenterId = employee.TransferCenterId;

                summary.TodayIncoming = await _cargoMovementRepository.CountAsync(x => x.TransferCenterId == transferCenterId && x.ToStatus == CargoStatus.InTransferCenter && x.MovementDate >= today);
                summary.TodayOutgoing = await _cargoMovementRepository.CountAsync(x => x.FromStatus == CargoStatus.InTransferCenter && x.ToStatus == CargoStatus.AtDestinationBranch && x.MovementDate >= today
                                                                                    && (x.Cargo.OriginBranch.TransferCenterId == transferCenterId || x.Cargo.DestinationBranch.TransferCenterId == transferCenterId));
                summary.OutForDelivery = await _cargoRepository.CountAsync(scope.And(x => x.CargoStatus == CargoStatus.OutForDelivery));
                summary.DeliveredToday = await _cargoRepository.CountAsync(scope.And(x => x.DeliveredDate >= today));
            }

            return summary;
        }

        public async Task<UserSummaryDto> GetUserSummaryAsync()
        {
            var customer = await _userContextService.GetCurrentCustomerAsync();
            var customerId = customer.Id;
            var active = CargoSlaRules.ActiveExpression();

            return new UserSummaryDto
            {
                FullName = $"{customer.FirstName} {customer.LastName}",
                TotalCargos = await _cargoRepository.CountAsync(x => x.SenderId == customerId || x.ReceiverId == customerId),
                ActiveCargos = await _cargoRepository.CountAsync(active.And(x => x.SenderId == customerId || x.ReceiverId == customerId)),
                IncomingActive = await _cargoRepository.CountAsync(active.And(x => x.ReceiverId == customerId)),
                OutgoingActive = await _cargoRepository.CountAsync(active.And(x => x.SenderId == customerId)),
                DeliveredCargos = await _cargoRepository.CountAsync(x => x.ReceiverId == customerId && x.CargoStatus == CargoStatus.Delivered)
            };
        }

        public async Task<PublicStatsDto> GetPublicStatsAsync()
        {
            return new PublicStatsDto
            {
                DeliveredCargos = await _cargoRepository.CountAsync(x => x.CargoStatus == CargoStatus.Delivered),
                ActiveBranches = await _branchRepository.CountAsync(x => x.IsActive),
                CoveredCities = await _cityRepository.CountAsync(),
                TransferCenters = await _transferCenterRepository.CountAsync(x => x.IsActive)
            };
        }

        public async Task<List<DailyCountDto>> GetDailyTrafficAsync(int days)
        {
            var from = DateTime.Today.AddDays(1 - days);
            var scope = await GetCargoScopeAsync();
            var rows = await _reportRepository.GetDailyCargoCountsAsync(from, scope);

            return Enumerable.Range(0, days)
                .Select(i => from.AddDays(i))
                .Select(date => new DailyCountDto
                {
                    Date = date,
                    Count = rows.FirstOrDefault(x => x.Date == date)?.Count ?? 0
                })
                .ToList();
        }

        public async Task<List<StatusCountDto>> GetStatusDistributionAsync()
        {
            var scope = await GetCargoScopeAsync();
            var rows = await _reportRepository.GetStatusDistributionAsync(scope);

            return rows.OrderBy(x => x.Status).Adapt<List<StatusCountDto>>();
        }

        public async Task<List<NamedCountDto>> GetTopBranchesAsync(int take)
        {
            EnsureAdmin();

            var to = DateTime.Today.AddDays(1);
            var rows = await _reportRepository.GetTopOriginBranchesAsync(take, to.AddDays(-TopBranchPeriodDays), to);

            return rows.Adapt<List<NamedCountDto>>();
        }

        public async Task<List<ResultCargoDto>> GetRecentCargosAsync(int take)
        {
            var result = await _cargoService.GetPagedAsync(new CargoFilterDto { Page = 1, PageSize = take });
            return result.Items;
        }

        public async Task<List<ResultCargoMovementDto>> GetRecentMovementsAsync(int take)
        {
            Expression<Func<CargoMovement, bool>> query = x => true;

            if (!_userContextService.IsAdmin)
            {
                var employee = await _userContextService.GetCurrentEmployeeAsync();

                if (employee.BranchId is not null)
                {
                    var branchId = employee.BranchId.Value;
                    query = x => x.BranchId == branchId || x.Cargo.OriginBranchId == branchId || x.Cargo.DestinationBranchId == branchId;
                }
                else
                {
                    var transferCenterId = employee.TransferCenterId;
                    query = x => x.TransferCenterId == transferCenterId;
                }
            }

            var movements = await _cargoMovementRepository.GetListAsync(query, q => q.OrderByDescending(x => x.MovementDate), take);
            return movements.Adapt<List<ResultCargoMovementDto>>();
        }

        public async Task<CargoDetailDto?> GetLastCargoAsync()
        {
            var recent = await GetRecentCargosAsync(1);
            var last = recent.FirstOrDefault();

            return last is null ? null : await _cargoService.GetDetailAsync(last.Id);
        }

        private async Task<Expression<Func<Cargo, bool>>> GetCargoScopeAsync()
        {
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
                    return x => x.OriginBranchId == branchId || x.DestinationBranchId == branchId;
                }

                var transferCenterId = employee.TransferCenterId;
                return x => x.OriginBranch.TransferCenterId == transferCenterId || x.DestinationBranch.TransferCenterId == transferCenterId;
            }

            var customer = await _userContextService.GetCurrentCustomerAsync();
            var customerId = customer.Id;
            return x => x.SenderId == customerId || x.ReceiverId == customerId;
        }

        private void EnsureAdmin()
        {
            if (!_userContextService.IsAdmin)
            {
                throw new BusinessException("Bu bilgilere yalnızca sistem yöneticisi erişebilir.");
            }
        }
    }
}
