using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.EmployeeDtos
{
    public class CreateEmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public EmployeePosition Position { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? TransferCenterId { get; set; }
    }
}