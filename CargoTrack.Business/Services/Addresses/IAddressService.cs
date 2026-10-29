using CargoTrack.DTO.DTOs.AddressDtos;

namespace CargoTrack.Business.Services.Addresses
{
    public interface IAddressService
    {
        Task<List<ResultAddressDto>> GetByCustomerIdAsync(Guid customerId);
        Task<List<ResultAddressDto>> GetMyAddressesAsync();
        Task<UpdateAddressDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateAddressDto createAddressDto);
        Task UpdateAsync(UpdateAddressDto updateAddressDto);
        Task DeleteAsync(Guid id);
    }
}
