using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DataAccess.Repositories.CargoPrices
{
    public interface ICargoPriceRepository : IRepository<CargoPrice>
    {
        Task<CargoPrice?> GetActivePriceAsync(DistanceZone distanceZone, decimal desi);
        Task<bool> HasOverlappingRangeAsync(DistanceZone distanceZone, decimal minDesi, decimal maxDesi, Guid? excludeCargoPriceId = null);
    }
}
