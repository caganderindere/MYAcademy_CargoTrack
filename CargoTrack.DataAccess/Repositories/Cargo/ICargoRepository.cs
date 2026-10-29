using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.Cargo
{
    using Cargo = CargoTrack.Entity.Entities.Cargo;

    public interface ICargoRepository : IRepository<Cargo>
    {
        Task<Cargo?> GetByTrackCodeAsync(string trackCode);
        Task AddMovementAsync(CargoMovement movement);
    }
}