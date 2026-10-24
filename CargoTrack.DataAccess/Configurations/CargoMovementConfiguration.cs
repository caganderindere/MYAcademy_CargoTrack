using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class CargoMovementConfiguration : IEntityTypeConfiguration<CargoMovement>
    {
        public void Configure(EntityTypeBuilder<CargoMovement> builder)
        {
            builder.Property(m => m.Description).HasMaxLength(500);
            builder.HasIndex(m => m.MovementDate);

            builder.HasOne(m => m.Cargo)
                   .WithMany(c => c.Movements)
                   .HasForeignKey(m => m.CargoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Branch)
                   .WithMany(b => b.Movements)
                   .HasForeignKey(m => m.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.TransferCenter)
                   .WithMany(t => t.Movements)
                   .HasForeignKey(m => m.TransferCenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Employee)
                   .WithMany(e => e.Movements)
                   .HasForeignKey(m => m.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
