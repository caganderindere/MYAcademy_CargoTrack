using CargoTrack.DTO.DTOs.CustomerDtos;

namespace CargoTrack.Business.Services.Customers
{
    public interface ICustomerService
    {
        Task<List<ResultCustomerDto>> GetAllAsync();
        Task<UpdateCustomerDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateCustomerDto createCustomerDto);
        Task UpdateAsync(UpdateCustomerDto updateCustomerDto);
        Task DeleteAsync(Guid id);
    }
}
