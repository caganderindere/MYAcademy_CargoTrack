using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class TransferCenterConfiguration : IEntityTypeConfiguration<TransferCenter>
    {
        public void Configure(EntityTypeBuilder<TransferCenter> builder)
        {
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Address).HasMaxLength(500);

            builder.HasOne(t => t.City)
                   .WithMany(c => c.TransferCenters)
                   .HasForeignKey(t => t.CityId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
