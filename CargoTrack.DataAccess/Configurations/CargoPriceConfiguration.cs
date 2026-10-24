using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class CargoPriceConfiguration : IEntityTypeConfiguration<CargoPrice>
    {
        public void Configure(EntityTypeBuilder<CargoPrice> builder)
        {
            builder.Property(p => p.MinDesi).HasPrecision(10, 2);
            builder.Property(p => p.MaxDesi).HasPrecision(10, 2);
            builder.Property(p => p.Price).HasPrecision(18, 2);
        }
    }
}
