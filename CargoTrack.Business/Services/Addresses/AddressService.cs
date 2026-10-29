using CargoTrack.Business.Exceptions;
using CargoTrack.DataAccess.Repositories.Addresses;
using CargoTrack.DataAccess.Repositories.Customers;
using CargoTrack.DTO.DTOs.AddressDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;

namespace CargoTrack.Business.Services.Addresses
{
    public class AddressService(IAddressRepository _addressRepository,
                                ICustomerRepository _customerRepository) : IAddressService
    {
        public async Task<List<ResultAddressDto>> GetByCustomerIdAsync(Guid customerId)
        {
            var addresses = await _addressRepository.GetByCustomerIdAsync(customerId);
            return addresses.Adapt<List<ResultAddressDto>>();
        }

        public async Task<UpdateAddressDto> GetByIdAsync(Guid id)
        {
            var address = await GetExistingAsync(id);
            return address.Adapt<UpdateAddressDto>();
        }

        public async Task CreateAsync(CreateAddressDto createAddressDto)
        {
            var customer = await _customerRepository.GetByIdAsync(createAddressDto.CustomerId);

            if (customer is null)
            {
                throw new BusinessException("Adres eklenmek istenen müşteri bulunamadı.");
            }

            var address = createAddressDto.Adapt<Address>();
            await _addressRepository.CreateAsync(address);
        }

        public async Task UpdateAsync(UpdateAddressDto updateAddressDto)
        {
            var address = await GetExistingAsync(updateAddressDto.Id);

            updateAddressDto.Adapt(address);
            await _addressRepository.UpdateAsync(address);
        }

        public async Task DeleteAsync(Guid id)
        {
            var address = await GetExistingAsync(id);

            var hasActiveCargo = address.Cargos.Any(x => x.CargoStatus != CargoStatus.Delivered
                                                      && x.CargoStatus != CargoStatus.ReturnedToSender);

            if (hasActiveCargo)
            {
                throw new BusinessException("Bu adrese giden ve süreci devam eden kargolar var. Adres şu an silinemez.");
            }

            await _addressRepository.DeleteAsync(address);
        }

        private async Task<Address> GetExistingAsync(Guid id)
        {
            var address = await _addressRepository.GetByIdAsync(id);

            if (address is null)
            {
                throw new BusinessException("Adres bulunamadı.");
            }

            return address;
        }
    }
}
