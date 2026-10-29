using CargoTrack.Business.Exceptions;
using CargoTrack.DataAccess.Repositories.Cities;
using CargoTrack.DTO.DTOs.CityDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Services.Cities
{
    public class CityService(ICityRepository _cityRepository) : ICityService
    {
        public async Task CreateAsync(CreateCityDto createCityDto)
        {
            var city = createCityDto.Adapt<City>();
            await _cityRepository.CreateAsync(city);
        }

        public async Task DeleteAsync(Guid id)
        {
            var city = await GetExistingAsync(id);

            if (city.Branches.Any() || city.TransferCenters.Any())
            {
                throw new BusinessException("Bu şehre bağlı şube veya transfer merkezi bulunduğu için silinemez.");
            }

            await _cityRepository.DeleteAsync(city);
        }

        public async Task<List<ResultCityDto>> GetAllAsync()
        {
            var cities = await _cityRepository.GetListAsync(x => true, q => q.OrderBy(x => x.Name));
            return cities.Adapt<List<ResultCityDto>>();
        }

        public async Task<UpdateCityDto> GetByIdAsync(Guid id)
        {
            var city = await GetExistingAsync(id);
            return city.Adapt<UpdateCityDto>();
        }

        public async Task UpdateAsync(UpdateCityDto updateCityDto)
        {
            var city = await GetExistingAsync(updateCityDto.Id);
            updateCityDto.Adapt(city);
            await _cityRepository.UpdateAsync(city);
        }

        private async Task<City> GetExistingAsync(Guid id)
        {
            return await _cityRepository.GetByIdAsync(id) ?? throw new BusinessException("Şehir bulunamadı.");
        }
    }
}
