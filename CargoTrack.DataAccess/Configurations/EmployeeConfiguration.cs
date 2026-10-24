using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);

            builder.HasOne(e => e.Branch)
                   .WithMany(b => b.Employees)
                   .HasForeignKey(e => e.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.TransferCenter)
                   .WithMany(t => t.Employees)
                   .HasForeignKey(e => e.TransferCenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.AppUser)
                   .WithOne(u => u.Employee)
                   .HasForeignKey<Employee>(e => e.AppUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
