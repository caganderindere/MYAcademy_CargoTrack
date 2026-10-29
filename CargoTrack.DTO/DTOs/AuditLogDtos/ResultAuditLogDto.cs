using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.AuditLogDtos
{
    public class ResultAuditLogDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UserName { get; set; }
        public AuditActionType ActionType { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? Description { get; set; }
    }
}
