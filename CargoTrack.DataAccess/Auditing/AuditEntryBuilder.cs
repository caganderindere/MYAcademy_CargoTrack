using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CargoTrack.DataAccess.Auditing
{
    public static class AuditEntryBuilder
    {
        private const string SystemUserName = "Sistem";
        private const string StatusPropertyName = nameof(Cargo.CargoStatus);

        private static readonly HashSet<string> IgnoredProperties = new()
        {
            nameof(BaseEntity.Id),
            nameof(BaseEntity.CreatedDate),
            nameof(BaseEntity.UpdatedDate),
            nameof(BaseEntity.IsDeleted)
        };

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter() }
        };

        public static List<AuditLog> Build(IEnumerable<EntityEntry<BaseEntity>> entries, IAuditUserProvider? userProvider)
        {
            var logs = new List<AuditLog>();

            foreach (var entry in entries)
            {
                if (entry.Entity is AuditLog || (entry.State != EntityState.Added && entry.State != EntityState.Modified))
                {
                    continue;
                }

                var log = entry.State == EntityState.Added ? BuildCreate(entry) : BuildModification(entry);

                if (log is null)
                {
                    continue;
                }

                log.UserId = userProvider?.UserId;
                log.UserName = userProvider?.UserName ?? SystemUserName;
                logs.Add(log);
            }

            return logs;
        }

        private static AuditLog BuildCreate(EntityEntry<BaseEntity> entry)
        {
            var values = entry.Properties
                .Where(p => !IgnoredProperties.Contains(p.Metadata.Name))
                .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

            return new AuditLog
            {
                ActionType = AuditActionType.Create,
                EntityName = entry.Metadata.ClrType.Name,
                EntityId = entry.Entity.Id.ToString(),
                NewValues = Serialize(values),
                Description = $"{entry.Metadata.ClrType.Name} kaydı oluşturuldu."
            };
        }

        private static AuditLog? BuildModification(EntityEntry<BaseEntity> entry)
        {
            var entityName = entry.Metadata.ClrType.Name;
            var isDeleted = entry.Property(nameof(BaseEntity.IsDeleted));

            if (isDeleted.CurrentValue is true && isDeleted.OriginalValue is false)
            {
                return new AuditLog
                {
                    ActionType = AuditActionType.Delete,
                    EntityName = entityName,
                    EntityId = entry.Entity.Id.ToString(),
                    Description = $"{entityName} kaydı silindi."
                };
            }

            var changed = entry.Properties
                .Where(p => !IgnoredProperties.Contains(p.Metadata.Name) && !Equals(p.OriginalValue, p.CurrentValue))
                .ToList();

            if (changed.Count == 0)
            {
                return null;
            }

            var statusProperty = changed.FirstOrDefault(p => p.Metadata.Name == StatusPropertyName);

            return new AuditLog
            {
                ActionType = statusProperty is null ? AuditActionType.Update : AuditActionType.StatusChange,
                EntityName = entityName,
                EntityId = entry.Entity.Id.ToString(),
                OldValues = Serialize(changed.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue)),
                NewValues = Serialize(changed.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue)),
                Description = statusProperty is null
                    ? $"{entityName} kaydı güncellendi: {string.Join(", ", changed.Select(p => p.Metadata.Name))}"
                    : $"Durum değişti: {statusProperty.OriginalValue} → {statusProperty.CurrentValue}"
            };
        }

        private static string Serialize(Dictionary<string, object?> values)
        {
            return JsonSerializer.Serialize(values, JsonOptions);
        }
    }
}
