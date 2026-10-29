using CargoTrack.DTO.DTOs.Common;

namespace CargoTrack.DTO.DTOs.BranchDtos
{
    public class BranchFilterDto : PageRequestDto
    {
        public Guid? CityId { get; set; }
    }
}
