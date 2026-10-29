using CargoTrack.Business.Constants;
using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Helpers;
using CargoTrack.Business.Services.CurrentUser;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.DataAccess.Repositories.Users;
using CargoTrack.DTO.DTOs.Common;
using CargoTrack.DTO.DTOs.UserDtos;
using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;

namespace CargoTrack.Business.Services.Users
{
    public class UserService(IAppUserRepository _appUserRepository,
                             IEmployeeRepository _employeeRepository,
                             UserManager<AppUser> _userManager,
                             RoleManager<AppRole> _roleManager,
                             IUserContextService _userContextService) : IUserService
    {
        public async Task<PagedResultDto<ResultUserDto>> GetPagedAsync(PageRequestDto request)
        {
            EnsureAdmin();

            var (page, pageSize) = request.Normalize();
            var paged = await _appUserRepository.GetPagedAsync(request.Search?.Trim(), page, pageSize);
            var result = paged.ToPagedResult<AppUser, ResultUserDto>();

            for (var i = 0; i < paged.Items.Count; i++)
            {
                result.Items[i].Roles = await _userManager.GetRolesAsync(paged.Items[i]);
            }

            return result;
        }

        public async Task<UserRolesDto> GetRolesAsync(Guid userId)
        {
            EnsureAdmin();

            var user = await GetUserAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.OrderBy(x => x.Name).ToList();

            return new UserRolesDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = user.UserName!,
                Roles = roles.Select(role => new RoleAssignDto
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    RoleName = role.Name!,
                    RoleExist = userRoles.Contains(role.Name!)
                }).ToList()
            };
        }

        public async Task AssignRolesAsync(UserRolesDto userRolesDto)
        {
            EnsureAdmin();

            var user = await GetUserAsync(userRolesDto.UserId);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var selected = userRolesDto.Roles.Where(x => x.RoleExist).Select(x => x.RoleName).ToList();

            if (user.Id == _userContextService.UserId && !selected.Contains(RoleNames.Admin))
            {
                throw new BusinessException("Kendi hesabınızın Admin rolünü kaldıramazsınız.");
            }

            if (selected.Contains(RoleNames.Manager) && await _employeeRepository.GetByAppUserIdAsync(user.Id) is null)
            {
                throw new BusinessException("Manager rolü yalnızca bir personel kaydına bağlı hesaplara verilebilir.");
            }

            var toAdd = selected.Except(currentRoles).ToList();
            var toRemove = currentRoles.Except(selected).ToList();

            if (toAdd.Count > 0)
            {
                await _userManager.AddToRolesAsync(user, toAdd);
            }

            if (toRemove.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, toRemove);
            }
        }

        private async Task<AppUser> GetUserAsync(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new BusinessException("Kullanıcı bulunamadı.");
        }

        private void EnsureAdmin()
        {
            if (!_userContextService.IsAdmin)
            {
                throw new BusinessException("Kullanıcı yönetimi yalnızca sistem yöneticisine açıktır.");
            }
        }
    }
}
