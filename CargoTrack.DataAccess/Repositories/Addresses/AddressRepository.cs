using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Addresses
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.Addresses
                .Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.Title)
                .ToListAsync();
        }
    }
}
