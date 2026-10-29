using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.Common
{
    public class StatusCountDto
    {
        public CargoStatus Status { get; set; }
        public int Count { get; set; }
    }
}
