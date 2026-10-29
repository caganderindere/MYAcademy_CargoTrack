using CargoTrack.DataAccess.Common;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.Users
{
    public interface IAppUserRepository
    {
        Task<PagedList<AppUser>> GetPagedAsync(string? search, int page, int pageSize);
        Task<int> CountAsync();
    }
}
