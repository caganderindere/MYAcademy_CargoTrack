using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(c => c.Email).HasMaxLength(100);
            builder.HasIndex(c => c.PhoneNumber);

            builder.HasOne(c => c.AppUser)
                   .WithOne(u => u.Customer)
                   .HasForeignKey<Customer>(c => c.AppUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
