using CargoTrack.Business.Exceptions;
using CargoTrack.DataAccess.Repositories.Abouts;
using CargoTrack.DTO.DTOs.AboutDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Services.Abouts
{
    public class AboutService(IAboutRepository _aboutRepository) : IAboutService
    {
        public async Task CreateAsync(CreateAboutDto createAboutDto)
        {
            var about = createAboutDto.Adapt<About>();
            await _aboutRepository.CreateAsync(about);
        }

        public async Task DeleteAsync(Guid id)
        {
            var about = await GetExistingAsync(id);
            await _aboutRepository.DeleteAsync(about);
        }

        public async Task<List<ResultAboutDto>> GetAllAsync()
        {
            var abouts = await _aboutRepository.GetAllAsync();
            return abouts.Adapt<List<ResultAboutDto>>();
        }

        public async Task<UpdateAboutDto> GetByIdAsync(Guid id)
        {
            var about = await GetExistingAsync(id);
            return about.Adapt<UpdateAboutDto>();
        }

        public async Task UpdateAsync(UpdateAboutDto updateAboutDto)
        {
            var about = await GetExistingAsync(updateAboutDto.Id);
            updateAboutDto.Adapt(about);
            await _aboutRepository.UpdateAsync(about);
        }

        private async Task<About> GetExistingAsync(Guid id)
        {
            return await _aboutRepository.GetByIdAsync(id) ?? throw new BusinessException("İçerik bulunamadı.");
        }
    }
}
