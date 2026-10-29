using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Customers
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber, Guid? excludeCustomerId = null)
        {
            return await _context.Customers.AnyAsync(x =>
                x.PhoneNumber == phoneNumber &&
                x.Id != excludeCustomerId);
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeCustomerId = null)
        {
            return await _context.Customers.AnyAsync(x =>
                x.Email == email &&
                x.Id != excludeCustomerId);
        }

        public async Task<Customer?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }

        public async Task<Customer?> GetByAppUserIdAsync(Guid appUserId)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.AppUserId == appUserId);
        }
    }
}
