using CargoTrack.DTO.DTOs.EmployeeDtos;

namespace CargoTrack.Business.Services.Employees
{
    public interface IEmployeeService
    {
        Task<List<ResultEmployeeDto>> GetAllAsync();
        Task<UpdateEmployeeDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateEmployeeDto createEmployeeDto);
        Task UpdateAsync(UpdateEmployeeDto updateEmployeeDto);
        Task DeleteAsync(Guid id);
        Task CreateAccountAsync(CreateEmployeeAccountDto createEmployeeAccountDto);
    }
}
