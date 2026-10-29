using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.Cargos
{
    public class DeliveryExceptionDto
    {
        public DeliveryExceptionReason Reason { get; set; }
        public string? Description { get; set; }
        public int AttemptNumber { get; set; }
        public DateTime OccurredDate { get; set; }
        public string? EmployeeFirstName { get; set; }
        public string? EmployeeLastName { get; set; }
        public string? EmployeeFullName => EmployeeFirstName is null ? null : $"{EmployeeFirstName} {EmployeeLastName}";
    }
}
