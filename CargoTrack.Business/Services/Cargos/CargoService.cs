using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Rules;
using CargoTrack.DataAccess.Repositories.Cargo;
using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;


namespace CargoTrack.Business.Services.Cargos
{
	public class CargoService(ICargoRepository _cargoRepository) : ICargoService
	{
		public async Task<string> CreateAsync(CreateCargoDto createCargoDto)
		{
			var cargo = createCargoDto.Adapt<Cargo>();

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

			cargo.CargoStatus = newStatus;

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
	}
}