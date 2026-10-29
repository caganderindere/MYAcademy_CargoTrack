using CargoTrack.DTO.DTOs.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.AuditLogDtos
{
    public class AuditLogFilterDto : PageRequestDto
    {
        public string? EntityName { get; set; }
        public AuditActionType? ActionType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
