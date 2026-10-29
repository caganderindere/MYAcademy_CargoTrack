using CargoTrack.DTO.DTOs.Cargos;

namespace CargoTrack.Business.Services.Cargos
{
    public interface ICargoService
    {
        Task<string> CreateAsync(CreateCargoDto createCargoDto);
    }
}