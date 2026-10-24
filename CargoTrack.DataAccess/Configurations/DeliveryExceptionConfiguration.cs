using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class DeliveryExceptionConfiguration : IEntityTypeConfiguration<DeliveryException>
    {
        public void Configure(EntityTypeBuilder<DeliveryException> builder)
        {
            builder.Property(d => d.Description).HasMaxLength(500);

            builder.HasOne(d => d.Cargo)
                   .WithMany(c => c.DeliveryExceptions)
                   .HasForeignKey(d => d.CargoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Employee)
                   .WithMany(e => e.DeliveryExceptions)
                   .HasForeignKey(d => d.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
