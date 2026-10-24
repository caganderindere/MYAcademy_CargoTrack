using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
            builder.Property(b => b.Address).HasMaxLength(500);
            builder.Property(b => b.PhoneNumber).HasMaxLength(20);

            builder.HasOne(b => b.City)
                   .WithMany(c => c.Branches)
                   .HasForeignKey(b => b.CityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.TransferCenter)
                   .WithMany(t => t.Branches)
                   .HasForeignKey(b => b.TransferCenterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
