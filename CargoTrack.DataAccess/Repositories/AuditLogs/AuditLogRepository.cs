using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.AuditLogs
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<string>> GetEntityNamesAsync()
        {
            return await _context.AuditLogs
                .Select(x => x.EntityName)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }
    }
}
