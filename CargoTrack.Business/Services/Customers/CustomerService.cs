using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Helpers;
using CargoTrack.DataAccess.Repositories.Customers;
using CargoTrack.DTO.DTOs.Common;
using CargoTrack.DTO.DTOs.CustomerDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;

namespace CargoTrack.Business.Services.Customers
{
    public class CustomerService(ICustomerRepository _customerRepository) : ICustomerService
    {
        private const int MinSearchLength = 2;
        private const int MaxLookupResults = 10;

        public async Task<List<ResultCustomerDto>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Adapt<List<ResultCustomerDto>>();
        }

        public async Task<PagedResultDto<ResultCustomerDto>> GetPagedAsync(PageRequestDto request)
        {
            var (page, pageSize) = request.Normalize();
            var search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim();

            var paged = await _customerRepository.GetPagedAsync(page, pageSize, x =>
                search == null
                || x.FirstName.Contains(search)
                || x.LastName.Contains(search)
                || x.PhoneNumber.Contains(search)
                || x.Email!.Contains(search));

            return paged.ToPagedResult<Customer, ResultCustomerDto>();
        }

        public async Task<List<CustomerLookupDto>> SearchAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < MinSearchLength)
            {
                return new List<CustomerLookupDto>();
            }

            var search = term.Trim();

            var customers = await _customerRepository.GetListAsync(
                x => x.FirstName.Contains(search) || x.LastName.Contains(search) || x.PhoneNumber.Contains(search),
                q => q.OrderBy(x => x.FirstName).ThenBy(x => x.LastName),
                MaxLookupResults);

            return customers.Adapt<List<CustomerLookupDto>>();
        }

        public async Task<ResultCustomerDto> GetSummaryAsync(Guid id)
        {
            var customer = await GetExistingAsync(id);
            return customer.Adapt<ResultCustomerDto>();
        }

        public async Task<UpdateCustomerDto> GetByIdAsync(Guid id)
        {
            var customer = await GetExistingAsync(id);
            return customer.Adapt<UpdateCustomerDto>();
        }

        public async Task CreateAsync(CreateCustomerDto createCustomerDto)
        {
            await CheckUniqueAsync(createCustomerDto.PhoneNumber, createCustomerDto.Email);

            var customer = createCustomerDto.Adapt<Customer>();
            await _customerRepository.CreateAsync(customer);
        }

        public async Task UpdateAsync(UpdateCustomerDto updateCustomerDto)
        {
            var customer = await GetExistingAsync(updateCustomerDto.Id);

            await CheckUniqueAsync(updateCustomerDto.PhoneNumber, updateCustomerDto.Email, updateCustomerDto.Id);

            updateCustomerDto.Adapt(customer);
            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await GetExistingAsync(id);

            var hasActiveCargo = customer.SentCargos.Any(x => !IsFinished(x.CargoStatus))
                              || customer.ReceivedCargos.Any(x => !IsFinished(x.CargoStatus));

            if (hasActiveCargo)
            {
                throw new BusinessException("Bu müşterinin süreci devam eden kargoları var. Kargolar tamamlanmadan müşteri silinemez.");
            }

            await _customerRepository.DeleteAsync(customer);
        }

        private async Task CheckUniqueAsync(string phoneNumber, string? email, Guid? excludeCustomerId = null)
        {
            if (await _customerRepository.PhoneNumberExistsAsync(phoneNumber, excludeCustomerId))
            {
                throw new BusinessException("Bu telefon numarası ile kayıtlı başka bir müşteri var.");
            }

            if (!string.IsNullOrWhiteSpace(email) && await _customerRepository.EmailExistsAsync(email, excludeCustomerId))
            {
                throw new BusinessException("Bu e-posta adresi ile kayıtlı başka bir müşteri var.");
            }
        }

        private static bool IsFinished(CargoStatus status)
        {
            return status == CargoStatus.Delivered || status == CargoStatus.ReturnedToSender;
        }

        private async Task<Customer> GetExistingAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer is null)
            {
                throw new BusinessException("Müşteri bulunamadı.");
            }

            return customer;
        }
    }
}
