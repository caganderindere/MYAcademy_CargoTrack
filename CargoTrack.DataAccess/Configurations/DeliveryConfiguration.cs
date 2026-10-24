using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.Property(d => d.ReceivedByName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Note).HasMaxLength(500);

            builder.HasOne(d => d.Cargo)
                   .WithOne(c => c.Delivery)
                   .HasForeignKey<Delivery>(d => d.CargoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Employee)
                   .WithMany(e => e.Deliveries)
                   .HasForeignKey(d => d.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
