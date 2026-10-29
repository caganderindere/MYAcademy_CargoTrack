using CargoTrack.Business.Exceptions;
using CargoTrack.DataAccess.Repositories.Branches;
using CargoTrack.DTO.DTOs.BranchDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;

namespace CargoTrack.Business.Services.Branches
{
    public class BranchService(IBranchRepository _branchRepository) : IBranchService
    {
        public async Task<List<ResultBranchDto>> GetAllAsync()
        {
            var branches = await _branchRepository.GetAllAsync();
            return branches.Adapt<List<ResultBranchDto>>();
        }

        public async Task<UpdateBranchDto> GetByIdAsync(Guid id)
        {
            var branch = await GetExistingAsync(id);
            return branch.Adapt<UpdateBranchDto>();
        }

        public async Task CreateAsync(CreateBranchDto createBranchDto)
        {
            var branch = createBranchDto.Adapt<Branch>();
            await _branchRepository.CreateAsync(branch);
        }

        public async Task UpdateAsync(UpdateBranchDto updateBranchDto)
        {
            var branch = await GetExistingAsync(updateBranchDto.Id);

            updateBranchDto.Adapt(branch);
            await _branchRepository.UpdateAsync(branch);
        }

        public async Task DeleteAsync(Guid id)
        {
            var branch = await GetExistingAsync(id);

            if (branch.Employees.Any())
            {
                throw new BusinessException("Bu şubede çalışan personeller var. Silmek yerine pasif yapabilirsiniz.");
            }

            var hasActiveCargo = branch.OriginCargos.Any(x => !IsFinished(x.CargoStatus))
                              || branch.DestinationCargos.Any(x => !IsFinished(x.CargoStatus));

            if (hasActiveCargo)
            {
                throw new BusinessException("Bu şubeden geçen ve süreci devam eden kargolar var. Şube şu an silinemez.");
            }

            await _branchRepository.DeleteAsync(branch);
        }

        private static bool IsFinished(CargoStatus status)
        {
            return status == CargoStatus.Delivered || status == CargoStatus.ReturnedToSender;
        }

        private async Task<Branch> GetExistingAsync(Guid id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);

            if (branch is null)
            {
                throw new BusinessException("Şube bulunamadı.");
            }

            return branch;
        }
    }
}
