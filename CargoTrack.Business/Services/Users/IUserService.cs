using CargoTrack.DTO.DTOs.Common;
using CargoTrack.DTO.DTOs.UserDtos;

namespace CargoTrack.Business.Services.Users
{
    public interface IUserService
    {
        Task<PagedResultDto<ResultUserDto>> GetPagedAsync(PageRequestDto request);
        Task<UserRolesDto> GetRolesAsync(Guid userId);
        Task AssignRolesAsync(UserRolesDto userRolesDto);
    }
}
