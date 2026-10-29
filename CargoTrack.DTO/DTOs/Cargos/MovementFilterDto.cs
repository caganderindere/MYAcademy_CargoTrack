using CargoTrack.DTO.DTOs.Common;

namespace CargoTrack.DTO.DTOs.Cargos
{
    public class MovementFilterDto : PageRequestDto
    {
        public DateTime? Date { get; set; }
    }
}
