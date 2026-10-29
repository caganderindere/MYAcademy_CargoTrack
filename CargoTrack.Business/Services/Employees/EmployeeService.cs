using CargoTrack.Business.Constants;
using CargoTrack.Business.Exceptions;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.DTO.DTOs.EmployeeDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace CargoTrack.Business.Services.Employees
{
    public class EmployeeService(IEmployeeRepository _employeeRepository,
                                 UserManager<AppUser> _userManager) : IEmployeeService
    {
        public async Task<List<ResultEmployeeDto>> GetAllAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Adapt<List<ResultEmployeeDto>>();
        }

        public async Task<UpdateEmployeeDto> GetByIdAsync(Guid id)
        {
            var employee = await GetExistingAsync(id);
            return employee.Adapt<UpdateEmployeeDto>();
        }

        public async Task CreateAsync(CreateEmployeeDto createEmployeeDto)
        {
            await CheckBranchManagerAsync(createEmployeeDto.Position, createEmployeeDto.BranchId);

            var employee = createEmployeeDto.Adapt<Employee>();
            await _employeeRepository.CreateAsync(employee);
        }

        public async Task UpdateAsync(UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = await GetExistingAsync(updateEmployeeDto.Id);

            await CheckBranchManagerAsync(updateEmployeeDto.Position, updateEmployeeDto.BranchId, updateEmployeeDto.Id);

            updateEmployeeDto.Adapt(employee);
            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await GetExistingAsync(id);

            if (employee.AppUserId is not null)
            {
                var user = await _userManager.FindByIdAsync(employee.AppUserId.Value.ToString());

                if (user is not null)
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                }
            }

            await _employeeRepository.DeleteAsync(employee);
        }

        public async Task CreateAccountAsync(CreateEmployeeAccountDto createEmployeeAccountDto)
        {
            var employee = await GetExistingAsync(createEmployeeAccountDto.EmployeeId);

            if (employee.AppUserId is not null)
            {
                throw new BusinessException("Bu personelin zaten bir kullanıcı hesabı var.");
            }

            var user = new AppUser
            {
                UserName = createEmployeeAccountDto.UserName,
                Email = createEmployeeAccountDto.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PhoneNumber = employee.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, createEmployeeAccountDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(x => x.Description));
                throw new BusinessException(errors);
            }

            await _userManager.AddToRoleAsync(user, RoleNames.Manager);

            employee.AppUserId = user.Id;
            await _employeeRepository.UpdateAsync(employee);
        }

        private async Task CheckBranchManagerAsync(EmployeePosition position, Guid? branchId, Guid? excludeEmployeeId = null)
        {
            if (position != EmployeePosition.BranchManager || branchId is null)
            {
                return;
            }

            if (await _employeeRepository.HasBranchManagerAsync(branchId.Value, excludeEmployeeId))
            {
                throw new BusinessException("Bu şubenin zaten bir yöneticisi var. Bir şubede yalnızca bir yönetici olabilir.");
            }
        }

        private async Task<Employee> GetExistingAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee is null)
            {
                throw new BusinessException("Personel bulunamadı.");
            }

            return employee;
        }
    }
}
