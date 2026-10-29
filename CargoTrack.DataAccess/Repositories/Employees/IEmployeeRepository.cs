using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.Employees
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<bool> HasBranchManagerAsync(Guid branchId, Guid? excludeEmployeeId = null);
        Task<Employee?> GetByAppUserIdAsync(Guid appUserId);
    }
}
