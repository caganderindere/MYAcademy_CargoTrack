using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class CargoConfiguration : IEntityTypeConfiguration<Cargo>
    {
        public void Configure(EntityTypeBuilder<Cargo> builder)
        {
            builder.Property(c => c.TrackCode).IsRequired().HasMaxLength(20);
            builder.HasIndex(c => c.TrackCode).IsUnique();

            builder.Property(c => c.Description).HasMaxLength(500);
            builder.Property(c => c.DeliveryCode).HasMaxLength(6);

            builder.Property(c => c.Weight).HasPrecision(10, 2);
            builder.Property(c => c.Width).HasPrecision(10, 2);
            builder.Property(c => c.Length).HasPrecision(10, 2);
            builder.Property(c => c.Height).HasPrecision(10, 2);
            builder.Property(c => c.Desi).HasPrecision(10, 2);
            builder.Property(c => c.Price).HasPrecision(18, 2);

            builder.HasIndex(c => c.CargoStatus);
            builder.HasIndex(c => c.EstimatedDeliveryDate);

            builder.HasOne(c => c.Sender)
                   .WithMany(m => m.SentCargos)
                   .HasForeignKey(c => c.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Receiver)
                   .WithMany(m => m.ReceivedCargos)
                   .HasForeignKey(c => c.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.DeliveryAddress)
                   .WithMany(m => m.Cargos)
                   .HasForeignKey(c => c.DeliveryAddressId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.OriginBranch)
                   .WithMany(m => m.OriginCargos)
                   .HasForeignKey(c => c.OriginBranchId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.DestinationBranch)
                   .WithMany(m => m.DestinationCargos)
                   .HasForeignKey(c => c.DestinationBranchId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
