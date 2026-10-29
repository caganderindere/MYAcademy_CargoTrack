using CargoTrack.Business.Exceptions;
using CargoTrack.DataAccess.Repositories.TransferCenters;
using CargoTrack.DTO.DTOs.TransferCenterDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Services.TransferCenters
{
    public class TransferCenterService(ITransferCenterRepository _transferCenterRepository) : ITransferCenterService
    {
        public async Task<List<ResultTransferCenterDto>> GetAllAsync()
        {
            var transferCenters = await _transferCenterRepository.GetAllAsync();
            return transferCenters.Adapt<List<ResultTransferCenterDto>>();
        }

        public async Task<UpdateTransferCenterDto> GetByIdAsync(Guid id)
        {
            var transferCenter = await GetExistingAsync(id);
            return transferCenter.Adapt<UpdateTransferCenterDto>();
        }

        public async Task CreateAsync(CreateTransferCenterDto createTransferCenterDto)
        {
            var transferCenter = createTransferCenterDto.Adapt<TransferCenter>();
            await _transferCenterRepository.CreateAsync(transferCenter);
        }

        public async Task UpdateAsync(UpdateTransferCenterDto updateTransferCenterDto)
        {
            var transferCenter = await GetExistingAsync(updateTransferCenterDto.Id);
            updateTransferCenterDto.Adapt(transferCenter);
            await _transferCenterRepository.UpdateAsync(transferCenter);
        }

        public async Task DeleteAsync(Guid id)
        {
            var transferCenter = await GetExistingAsync(id);

            if (transferCenter.Branches.Any())
            {
                throw new BusinessException("Bu transfer merkezine bağlı şubeler var. Silmek yerine pasif yapabilirsiniz.");
            }

            await _transferCenterRepository.DeleteAsync(transferCenter);
        }

        private async Task<TransferCenter> GetExistingAsync(Guid id)
        {
            var transferCenter = await _transferCenterRepository.GetByIdAsync(id);

            if (transferCenter is null)
            {
                throw new BusinessException("Transfer merkezi bulunamadı.");
            }

            return transferCenter;
        }
    }
}