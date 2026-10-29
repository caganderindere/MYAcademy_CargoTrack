using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.EmployeeDtos
{
    public class ResultEmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public EmployeePosition Position { get; set; }
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public Guid? TransferCenterId { get; set; }
        public string? TransferCenterName { get; set; }
        public Guid? AppUserId { get; set; }
        public bool HasAccount => AppUserId.HasValue;
        public string FullName => $"{FirstName} {LastName}";
        public string? UnitName => BranchName ?? TransferCenterName;
    }
}
