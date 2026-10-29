using CargoTrack.Business.Exceptions;
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
							  ICargoPricingService _cargoPricingService) : ICargoService
	{
		private const int MaxFailedDeliveryAttempts = 3;

		public async Task<string> CreateAsync(CreateCargoDto createCargoDto)
		{
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
					Description = "Gönderi oluþturuldu."
				}
			};

			await _cargoRepository.CreateAsync(cargo);

			return cargo.TrackCode;
		}

		public async Task ChangeStatusAsync(ChangeCargoStatusDto changeCargoStatusDto)
		{
			var cargo = await _cargoRepository.GetByIdAsync(changeCargoStatusDto.CargoId);

			if (cargo is null)
			{
				throw new BusinessException("Kargo bulunamadý.");
			}

			var oldStatus = cargo.CargoStatus;
			var newStatus = changeCargoStatusDto.NewStatus;

			if (!CargoStatusRules.CanTransition(oldStatus, newStatus))
			{
				throw new BusinessException($"Kargo '{oldStatus}' durumundan '{newStatus}' durumuna geçemez.");
			}

			if (newStatus == CargoStatus.Delivered)
			{
				throw new BusinessException("Teslim iþlemi, teslimat kodu doðrulanarak yapýlmalýdýr.");
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
				EmployeeId = changeCargoStatusDto.EmployeeId
			};

			await _cargoRepository.AddMovementAsync(movement);
		}

		public async Task DeliverAsync(DeliverCargoDto deliverCargoDto)
		{
			var cargo = await _cargoRepository.GetByIdAsync(deliverCargoDto.CargoId);

			if (cargo is null)
			{
				throw new BusinessException("Kargo bulunamadý.");
			}

			if (cargo.CargoStatus != CargoStatus.OutForDelivery)
			{
				throw new BusinessException("Sadece daðýtýmdaki kargolar teslim edilebilir.");
			}

			if (cargo.DeliveryCode != deliverCargoDto.DeliveryCode)
			{
				throw new BusinessException("Teslimat kodu hatalý.");
			}

			var now = DateTime.Now;

			cargo.CargoStatus = CargoStatus.Delivered;
			cargo.DeliveredDate = now;
			cargo.DeliveryCode = null;

			var delivery = new Delivery
			{
				CargoId = cargo.Id,
				EmployeeId = deliverCargoDto.EmployeeId,
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
				EmployeeId = deliverCargoDto.EmployeeId
			};

			await _cargoRepository.AddDeliveryAsync(delivery, movement);
		}

		public async Task ReportFailedDeliveryAsync(FailedDeliveryDto failedDeliveryDto)
		{
			var cargo = await _cargoRepository.GetByIdAsync(failedDeliveryDto.CargoId);

			if (cargo is null)
			{
				throw new BusinessException("Kargo bulunamadý.");
			}

			if (cargo.CargoStatus != CargoStatus.OutForDelivery)
			{
				throw new BusinessException("Sadece daðýtýmdaki kargolar için teslimat sorunu bildirilebilir.");
			}

			var now = DateTime.Now;

			cargo.FailedDeliveryAttempts++;
			cargo.CargoStatus = CargoStatus.DeliveryFailed;
			cargo.DeliveryCode = null;

			var deliveryException = new DeliveryException
			{
				CargoId = cargo.Id,
				EmployeeId = failedDeliveryDto.EmployeeId,
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
					EmployeeId = failedDeliveryDto.EmployeeId
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
					Description = $"{MaxFailedDeliveryAttempts} baþarýsýz teslimat denemesi sonrasý iade süreci baþlatýldý.",
					BranchId = cargo.DestinationBranchId
				});
			}

			await _cargoRepository.AddDeliveryExceptionAsync(deliveryException, movements);
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