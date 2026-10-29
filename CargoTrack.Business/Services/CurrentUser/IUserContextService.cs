using CargoTrack.Entity.Entities;

namespace CargoTrack.Business.Services.CurrentUser
{
    public interface IUserContextService
    {
        bool IsAdmin { get; }
        bool IsManager { get; }
        Task<Employee> GetCurrentEmployeeAsync();
        Task<Customer> GetCurrentCustomerAsync();
    }
}
