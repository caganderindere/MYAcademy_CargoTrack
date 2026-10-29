using CargoTrack.DTO.DTOs.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.Cargos
{
    public class CargoFilterDto : PageRequestDto
    {
        public CargoStatus? Status { get; set; }
        public Guid? BranchId { get; set; }
        public string? Direction { get; set; }
        public bool OnlyDelayed { get; set; }
        public bool OnlyActive { get; set; }
        public bool OnlyProblematic { get; set; }
        public bool OnlyFinished { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
