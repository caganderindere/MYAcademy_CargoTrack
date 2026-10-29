using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.Cargos
{
    public class ResultCargoMovementDto
    {
        public Guid Id { get; set; }
        public Guid CargoId { get; set; }
        public string CargoTrackCode { get; set; }
        public CargoStatus? FromStatus { get; set; }
        public CargoStatus ToStatus { get; set; }
        public DateTime MovementDate { get; set; }
        public string? Description { get; set; }
        public string? BranchName { get; set; }
        public string? TransferCenterName { get; set; }
        public string? EmployeeFirstName { get; set; }
        public string? EmployeeLastName { get; set; }
        public string? EmployeeFullName => EmployeeFirstName is null ? null : $"{EmployeeFirstName} {EmployeeLastName}";
        public string? LocationName => BranchName ?? TransferCenterName;
    }
}
