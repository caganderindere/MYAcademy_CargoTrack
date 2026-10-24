using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
