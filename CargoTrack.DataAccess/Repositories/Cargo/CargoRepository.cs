using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Cargo
{
    using Cargo = CargoTrack.Entity.Entities.Cargo;

    public class CargoRepository : GenericRepository<Cargo>, ICargoRepository
    {
        private readonly AppDbContext _context;

        public CargoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cargo?> GetByTrackCodeAsync(string trackCode)
        {
            return await _context.Cargos.FirstOrDefaultAsync(x => x.TrackCode == trackCode);
        }

        public async Task<List<Cargo>> GetByBranchIdAsync(Guid branchId)
        {
            return await _context.Cargos
                .Where(x => x.OriginBranchId == branchId || x.DestinationBranchId == branchId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Cargo>> GetByTransferCenterIdAsync(Guid transferCenterId)
        {
            return await _context.Cargos
                .Where(x => x.OriginBranch.TransferCenterId == transferCenterId
                         || x.DestinationBranch.TransferCenterId == transferCenterId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Cargo>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.Cargos
                .Where(x => x.SenderId == customerId || x.ReceiverId == customerId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task AddMovementAsync(CargoMovement movement)
        {
            await _context.CargoMovements.AddAsync(movement);
            await _context.SaveChangesAsync();
        }

        public async Task AddDeliveryAsync(Delivery delivery, CargoMovement movement)
        {
            await _context.Deliveries.AddAsync(delivery);
            await _context.CargoMovements.AddAsync(movement);
            await _context.SaveChangesAsync();
        }

        public async Task AddDeliveryExceptionAsync(DeliveryException deliveryException, List<CargoMovement> movements)
        {
            await _context.DeliveryExceptions.AddAsync(deliveryException);
            await _context.CargoMovements.AddRangeAsync(movements);
            await _context.SaveChangesAsync();
        }
    }
}
