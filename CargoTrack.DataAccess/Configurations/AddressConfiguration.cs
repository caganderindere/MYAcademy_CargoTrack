using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.Property(a => a.Title).IsRequired().HasMaxLength(50);
            builder.Property(a => a.District).IsRequired().HasMaxLength(50);
            builder.Property(a => a.FullAddress).IsRequired().HasMaxLength(500);

            builder.HasOne(a => a.City)
                   .WithMany(c => c.Addresses)
                   .HasForeignKey(a => a.CityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Customer)
                   .WithMany(c => c.Addresses)
                   .HasForeignKey(a => a.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
