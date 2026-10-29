using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Employees
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> HasBranchManagerAsync(Guid branchId, Guid? excludeEmployeeId = null)
        {
            return await _context.Employees.AnyAsync(x =>
                x.BranchId == branchId &&
                x.Position == EmployeePosition.BranchManager &&
                x.Id != excludeEmployeeId);
        }

        public async Task<Employee?> GetByAppUserIdAsync(Guid appUserId)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.AppUserId == appUserId);
        }
    }
}
