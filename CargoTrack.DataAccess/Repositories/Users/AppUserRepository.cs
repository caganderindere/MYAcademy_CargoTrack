using CargoTrack.DataAccess.Common;
using CargoTrack.DataAccess.Context;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Users
{
    public class AppUserRepository(AppDbContext _context) : IAppUserRepository
    {
        public async Task<PagedList<AppUser>> GetPagedAsync(string? search, int page, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.UserName!.Contains(search)
                                      || x.Email!.Contains(search)
                                      || x.FirstName.Contains(search)
                                      || x.LastName.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<AppUser>(items, totalCount, page, pageSize);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Users.CountAsync();
        }
    }
}
