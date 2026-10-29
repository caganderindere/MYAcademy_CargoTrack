using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.Customers
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<bool> PhoneNumberExistsAsync(string phoneNumber, Guid? excludeCustomerId = null);
        Task<bool> EmailExistsAsync(string email, Guid? excludeCustomerId = null);
        Task<Customer?> GetByPhoneNumberAsync(string phoneNumber);
        Task<Customer?> GetByAppUserIdAsync(Guid appUserId);
    }
}
