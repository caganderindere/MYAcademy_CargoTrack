using CargoTrack.DTO.DTOs.Common;
using CargoTrack.DTO.DTOs.EmployeeDtos;

namespace CargoTrack.Business.Services.Employees
{
    public interface IEmployeeService
    {
        Task<List<ResultEmployeeDto>> GetAllAsync();
        Task<PagedResultDto<ResultEmployeeDto>> GetPagedAsync(EmployeeFilterDto filter);
        Task<ResultEmployeeDto> GetSummaryAsync(Guid id);
        Task<UpdateEmployeeDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateEmployeeDto createEmployeeDto);
        Task UpdateAsync(UpdateEmployeeDto updateEmployeeDto);
        Task DeleteAsync(Guid id);
        Task CreateAccountAsync(CreateEmployeeAccountDto createEmployeeAccountDto);
    }
}
