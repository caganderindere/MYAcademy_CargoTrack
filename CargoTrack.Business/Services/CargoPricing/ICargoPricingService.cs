using CargoTrack.DTO.DTOs.Cargos;

namespace CargoTrack.Business.Services.CargoPricing
{
    public interface ICargoPricingService
    {
        Task<(decimal Desi, decimal Price)> CalculateAsync(CreateCargoDto createCargoDto);
    }
}