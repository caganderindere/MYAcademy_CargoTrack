using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.CargoPriceDtos
{
    public class UpdateCargoPriceDto
    {
        public Guid Id { get; set; }
        public decimal MinDesi { get; set; }
        public decimal MaxDesi { get; set; }
        public DistanceZone DistanceZone { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
