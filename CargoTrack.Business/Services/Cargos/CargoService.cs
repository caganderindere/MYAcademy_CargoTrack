using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Services.CurrentUser;
using CargoTrack.Business.Rules;
using CargoTrack.Business.Services.CargoPricing;
using CargoTrack.DataAccess.Repositories.Cargo;
using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;
using System.Security.Cryptography;

namespace CargoTrack.Business.Services.Cargos
{
	public class CargoService(ICargoRepository _cargoRepository,
							  ICargoPricingService _cargoPricingService,
						  IUserContextService _userContextService) : ICargoService
	{
		private const int MaxFailedDeliveryAttempts = 3;
		private const string UnauthorizedMessage = "Bu işlem için yetkiniz yok.";

		public async Task<List<ResultCargoDto>> GetListAsync()
		{
			List<Cargo> cargos;

			if (_userContextService.IsAdmin)
			{
				cargos = (await _cargoRepository.GetAllAsync()).OrderByDescending(x => x.CreatedDate).ToList();
			}
			else if (_userContextService.IsManager)
			{
				var employee = await _userContextService.GetCurrentEmployeeAsync();

				cargos = employee.BranchId is not null
					? await _cargoRepository.GetByBranchIdAsync(employee.BranchId.Value)
					: await _cargoRepository.GetByTransferCenterIdAsync(employee.TransferCenterId!.Value);
			}
			else
			{
				var customer = await _userContextService.GetCurrentCustomerAsync();
				cargos = await _cargoRepository.GetByCustomerIdAsync(customer.Id);
			}

			return cargos.Adapt<List<ResultCargoDto>>();
		}

		public async Task<string> CreateAsync(CreateCargoDto createCargoDto)
		{
			await EnsureCanCreateAsync(createCargoDto.OriginBranchId);

			var (desi, price) = await _cargoPricingService.CalculateAsync(createCargoDto);

			var cargo = createCargoDto.Adapt<Cargo>();

			cargo.Desi = desi;
			cargo.Price = price;
			cargo.EstimatedDeliveryDate = DeliveryDateRules.CalculateEstimatedDeliveryDate(DateTime.Now, cargo.DeliveryType, cargo.AdditionalServices);
			cargo.TrackCode = await GenerateTrackCodeAsync();
			cargo.CargoStatus = CargoStatus.Created;

			cargo.Movements = new List<CargoMovement>
			{
				new CargoMovement
				{
					FromStatus = null,
					ToStatus = CargoStatus.Created,
					MovementDate = DateTime.Now,
					BranchId = cargo.OriginBranchId,
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
			var cargo = await _cargoRepository.GetByIdAsync(cargoId);

			if (cargo is null)
			{
				throw new BusinessException("Kargo bulunamadı.");
			}

			if (_userContextService.IsAdmin)
			{
				return cargo;
			}

			if (!_userContextService.IsManager)
			{
				throw new BusinessException(UnauthorizedMessage);
			}

			var employee = await _userContextService.GetCurrentEmployeeAsync();

			var canManage = employee.BranchId is not null
				? cargo.OriginBranchId == employee.BranchId || cargo.DestinationBranchId == employee.BranchId
				: cargo.OriginBranch.TransferCenterId == employee.TransferCenterId || cargo.DestinationBranch.TransferCenterId == employee.TransferCenterId;

			if (!canManage)
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