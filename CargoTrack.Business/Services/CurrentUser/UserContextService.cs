using CargoTrack.Business.Constants;
using CargoTrack.Business.Exceptions;
using CargoTrack.DataAccess.Repositories.Customers;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.Entity.Entities;

namespace CargoTrack.Business.Services.CurrentUser
{
    public class UserContextService(ICurrentUserService _currentUserService,
                                    IEmployeeRepository _employeeRepository,
                                    ICustomerRepository _customerRepository) : IUserContextService
    {
        private Employee? _currentEmployee;
        private Customer? _currentCustomer;

        public Guid? UserId => _currentUserService.UserId;

        public bool IsAdmin => _currentUserService.IsInRole(RoleNames.Admin);

        public bool IsManager => _currentUserService.IsInRole(RoleNames.Manager);

        public bool IsCustomer => _currentUserService.IsInRole(RoleNames.User);

        public async Task<Employee> GetCurrentEmployeeAsync()
        {
            if (_currentEmployee is not null)
            {
                return _currentEmployee;
            }

            var userId = GetRequiredUserId();

            _currentEmployee = await _employeeRepository.GetByAppUserIdAsync(userId)
                ?? throw new BusinessException("Hesabınız bir personel kaydına bağlı değil.");

            return _currentEmployee;
        }

        public async Task<Customer> GetCurrentCustomerAsync()
        {
            if (_currentCustomer is not null)
            {
                return _currentCustomer;
            }

            var userId = GetRequiredUserId();

            _currentCustomer = await _customerRepository.GetByAppUserIdAsync(userId)
                ?? throw new BusinessException("Hesabınız bir müşteri kaydına bağlı değil.");

            return _currentCustomer;
        }

        private Guid GetRequiredUserId()
        {
            return _currentUserService.UserId
                ?? throw new BusinessException("Bu işlem için giriş yapmalısınız.");
        }
    }
}
