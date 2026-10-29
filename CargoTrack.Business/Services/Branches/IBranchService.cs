using CargoTrack.DTO.DTOs.BranchDtos;
using CargoTrack.DTO.DTOs.Common;

namespace CargoTrack.Business.Services.Branches
{
    public interface IBranchService
    {
        Task<List<ResultBranchDto>> GetAllAsync();
        Task<List<ResultBranchDto>> GetActiveAsync();
        Task<PagedResultDto<ResultBranchDto>> GetPagedAsync(BranchFilterDto filter);
        Task<UpdateBranchDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateBranchDto createBranchDto);
        Task UpdateAsync(UpdateBranchDto updateBranchDto);
        Task DeleteAsync(Guid id);
    }
}
