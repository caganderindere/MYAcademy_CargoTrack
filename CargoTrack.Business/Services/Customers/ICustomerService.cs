using CargoTrack.DTO.DTOs.Common;
using CargoTrack.DTO.DTOs.CustomerDtos;

namespace CargoTrack.Business.Services.Customers
{
    public interface ICustomerService
    {
        Task<List<ResultCustomerDto>> GetAllAsync();
        Task<PagedResultDto<ResultCustomerDto>> GetPagedAsync(PageRequestDto request);
        Task<List<CustomerLookupDto>> SearchAsync(string term);
        Task<ResultCustomerDto> GetSummaryAsync(Guid id);
        Task<UpdateCustomerDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateCustomerDto createCustomerDto);
        Task UpdateAsync(UpdateCustomerDto updateCustomerDto);
        Task DeleteAsync(Guid id);
    }
}
