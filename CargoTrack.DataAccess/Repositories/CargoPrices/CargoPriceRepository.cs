using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.CargoPrices
{
    public class CargoPriceRepository : GenericRepository<CargoPrice>, ICargoPriceRepository
    {
        private readonly AppDbContext _context;

        public CargoPriceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CargoPrice?> GetActivePriceAsync(DistanceZone distanceZone, decimal desi)
        {
            return await _context.CargoPrices
                .Where(x => x.IsActive
                         && x.DistanceZone == distanceZone
                         && x.MinDesi < desi
                         && desi <= x.MaxDesi)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasOverlappingRangeAsync(DistanceZone distanceZone, decimal minDesi, decimal maxDesi, Guid? excludeCargoPriceId = null)
        {
            return await _context.CargoPrices.AnyAsync(x =>
                x.IsActive &&
                x.DistanceZone == distanceZone &&
                x.Id != excludeCargoPriceId &&
                minDesi < x.MaxDesi &&
                x.MinDesi < maxDesi);
        }
    }
}
