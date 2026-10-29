using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.Entity.Entities;

namespace CargoTrack.Business.Services.Cargos
{
    public interface ICargoService
    {
        Task<string> CreateAsync(CreateCargoDto createCargoDto);
        Task ChangeStatusAsync(ChangeCargoStatusDto changeCargoStatusDto);

        Task AddDeliveryAsync(Delivery delivery, CargoMovement movement);
    }
}