using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.CargoPriceDtos
{
    public class CreateCargoPriceDto
    {
        public decimal MinDesi { get; set; }
        public decimal MaxDesi { get; set; }
        public DistanceZone DistanceZone { get; set; }
        public decimal Price { get; set; }
    }
}
