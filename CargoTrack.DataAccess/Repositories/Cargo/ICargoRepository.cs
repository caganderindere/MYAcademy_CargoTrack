using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.Cargo
{
    using Cargo = CargoTrack.Entity.Entities.Cargo;

    public interface ICargoRepository : IRepository<Cargo>
    {
        Task<Cargo?> GetByTrackCodeAsync(string trackCode);
        Task<List<Cargo>> GetByBranchIdAsync(Guid branchId);
        Task<List<Cargo>> GetByTransferCenterIdAsync(Guid transferCenterId);
        Task<List<Cargo>> GetByCustomerIdAsync(Guid customerId);
        Task AddMovementAsync(CargoMovement movement);
        Task AddDeliveryAsync(Delivery delivery, CargoMovement movement);
        Task AddDeliveryExceptionAsync(DeliveryException deliveryException, List<CargoMovement> movements);
    }
}
