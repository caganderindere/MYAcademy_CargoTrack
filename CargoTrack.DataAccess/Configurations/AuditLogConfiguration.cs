using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoTrack.DataAccess.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.Property(a => a.UserName).HasMaxLength(256);
            builder.Property(a => a.EntityName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.EntityId).IsRequired().HasMaxLength(50);
            builder.Property(a => a.Description).HasMaxLength(500);

            builder.HasIndex(a => new { a.EntityName, a.EntityId });
            builder.HasIndex(a => a.CreatedDate);
        }
    }
}
