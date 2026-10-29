using CargoTrack.DTO.DTOs.CargoPriceDtos;

namespace CargoTrack.Business.Services.CargoPrices
{
    public interface ICargoPriceService
    {
        Task<List<ResultCargoPriceDto>> GetAllAsync();
        Task<UpdateCargoPriceDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateCargoPriceDto createCargoPriceDto);
        Task UpdateAsync(UpdateCargoPriceDto updateCargoPriceDto);
        Task DeleteAsync(Guid id);
    }
}
