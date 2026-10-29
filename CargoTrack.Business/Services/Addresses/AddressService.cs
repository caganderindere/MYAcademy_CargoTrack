using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Rules;
using CargoTrack.Business.Services.CurrentUser;
using CargoTrack.DataAccess.Repositories.Addresses;
using CargoTrack.DataAccess.Repositories.Customers;
using CargoTrack.DTO.DTOs.AddressDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Services.Addresses
{
    public class AddressService(IAddressRepository _addressRepository,
                                ICustomerRepository _customerRepository,
                                IUserContextService _userContextService) : IAddressService
    {
        public async Task<List<ResultAddressDto>> GetByCustomerIdAsync(Guid customerId)
        {
            await EnsureCanAccessCustomerAsync(customerId);

            var addresses = await _addressRepository.GetByCustomerIdAsync(customerId);
            return addresses.Adapt<List<ResultAddressDto>>();
        }

        public async Task<List<ResultAddressDto>> GetMyAddressesAsync()
        {
            var customer = await _userContextService.GetCurrentCustomerAsync();
            var addresses = await _addressRepository.GetByCustomerIdAsync(customer.Id);
            return addresses.Adapt<List<ResultAddressDto>>();
        }

        public async Task<UpdateAddressDto> GetByIdAsync(Guid id)
        {
            var address = await GetExistingAsync(id);
            return address.Adapt<UpdateAddressDto>();
        }

        public async Task CreateAsync(CreateAddressDto createAddressDto)
        {
            if (IsCustomerOnly())
            {
                createAddressDto.CustomerId = (await _userContextService.GetCurrentCustomerAsync()).Id;
            }

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

            var hasActiveCargo = address.Cargos.Any(x => !CargoSlaRules.IsFinished(x.CargoStatus));

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

            await EnsureCanAccessCustomerAsync(address.CustomerId);

            return address;
        }

        private async Task EnsureCanAccessCustomerAsync(Guid customerId)
        {
            if (!IsCustomerOnly())
            {
                return;
            }

            var customer = await _userContextService.GetCurrentCustomerAsync();

            if (customer.Id != customerId)
            {
                throw new BusinessException("Bu adres bilgilerine erişim yetkiniz yok.");
            }
        }

        private bool IsCustomerOnly()
        {
            return !_userContextService.IsAdmin && !_userContextService.IsManager;
        }
    }
}
