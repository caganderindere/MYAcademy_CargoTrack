using CargoTrack.DTO.DTOs.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.EmployeeDtos
{
    public class EmployeeFilterDto : PageRequestDto
    {
        public EmployeePosition? Position { get; set; }
        public Guid? BranchId { get; set; }
    }
}
