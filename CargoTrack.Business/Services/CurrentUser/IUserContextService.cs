using CargoTrack.Entity.Entities;

namespace CargoTrack.Business.Services.CurrentUser
{
    public interface IUserContextService
    {
        Guid? UserId { get; }
        bool IsAdmin { get; }
        bool IsManager { get; }
        bool IsCustomer { get; }
        Task<Employee> GetCurrentEmployeeAsync();
        Task<Customer> GetCurrentCustomerAsync();
    }
}
