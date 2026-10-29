using CargoTrack.DataAccess.Auditing;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        private readonly IAuditUserProvider? _auditUserProvider;

        public AppDbContext(DbContextOptions options, IAuditUserProvider? auditUserProvider = null) : base(options)
        {
            _auditUserProvider = auditUserProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<About>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Address>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Branch>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Cargo>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<CargoMovement>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<CargoPrice>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<City>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<ContactInfo>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Customer>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Delivery>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<DeliveryException>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Employee>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<TransferCenter>().HasQueryFilter(x => !x.IsDeleted);
        }

        public override int SaveChanges()
        {
            SetAuditFields();
            AddAuditLogs();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditFields();
            AddAuditLogs();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditLogs()
        {
            var logs = AuditEntryBuilder.Build(ChangeTracker.Entries<BaseEntity>().ToList(), _auditUserProvider);

            if (logs.Count > 0)
            {
                AuditLogs.AddRange(logs);
            }
        }

        private void SetAuditFields()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.Now;
                    entry.Property(x => x.CreatedDate).IsModified = false;
                }
            }
        }

        public DbSet<About> Abouts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<CargoMovement> CargoMovements { get; set; }
        public DbSet<CargoPrice> CargoPrices { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryException> DeliveryExceptions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<TransferCenter> TransferCenters { get; set; }
    }
}