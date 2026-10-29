using CargoTrack.Business.Exceptions;
using CargoTrack.DataAccess.Repositories.CargoPrices;
using CargoTrack.DTO.DTOs.CargoPriceDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;

namespace CargoTrack.Business.Services.CargoPrices
{
    public class CargoPriceService(ICargoPriceRepository _cargoPriceRepository) : ICargoPriceService
    {
        public async Task<List<ResultCargoPriceDto>> GetAllAsync()
        {
            var cargoPrices = await _cargoPriceRepository.GetAllAsync();

            return cargoPrices.OrderBy(x => x.DistanceZone)
                              .ThenBy(x => x.MinDesi)
                              .Adapt<List<ResultCargoPriceDto>>();
        }

        public async Task<UpdateCargoPriceDto> GetByIdAsync(Guid id)
        {
            var cargoPrice = await GetExistingAsync(id);
            return cargoPrice.Adapt<UpdateCargoPriceDto>();
        }

        public async Task CreateAsync(CreateCargoPriceDto createCargoPriceDto)
        {
            await CheckOverlapAsync(createCargoPriceDto.DistanceZone, createCargoPriceDto.MinDesi, createCargoPriceDto.MaxDesi);

            var cargoPrice = createCargoPriceDto.Adapt<CargoPrice>();
            await _cargoPriceRepository.CreateAsync(cargoPrice);
        }

        public async Task UpdateAsync(UpdateCargoPriceDto updateCargoPriceDto)
        {
            var cargoPrice = await GetExistingAsync(updateCargoPriceDto.Id);

            if (updateCargoPriceDto.IsActive)
            {
                await CheckOverlapAsync(updateCargoPriceDto.DistanceZone, updateCargoPriceDto.MinDesi, updateCargoPriceDto.MaxDesi, updateCargoPriceDto.Id);
            }

            updateCargoPriceDto.Adapt(cargoPrice);
            await _cargoPriceRepository.UpdateAsync(cargoPrice);
        }

        public async Task DeleteAsync(Guid id)
        {
            var cargoPrice = await GetExistingAsync(id);
            await _cargoPriceRepository.DeleteAsync(cargoPrice);
        }

        private async Task CheckOverlapAsync(DistanceZone distanceZone, decimal minDesi, decimal maxDesi, Guid? excludeCargoPriceId = null)
        {
            if (await _cargoPriceRepository.HasOverlappingRangeAsync(distanceZone, minDesi, maxDesi, excludeCargoPriceId))
            {
                throw new BusinessException("Bu mesafe bölgesinde bu desi aralığıyla çakışan aktif bir tarife zaten var.");
            }
        }

        private async Task<CargoPrice> GetExistingAsync(Guid id)
        {
            var cargoPrice = await _cargoPriceRepository.GetByIdAsync(id);

            if (cargoPrice is null)
            {
                throw new BusinessException("Tarife bulunamadı.");
            }

            return cargoPrice;
        }
    }
}
