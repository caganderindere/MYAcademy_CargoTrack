using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities
{
    public class CargoPrice: BaseEntity
    {
        public decimal MinDesi { get; set; }
        public decimal MaxDesi { get; set; }
        public DistanceZone DistanceZone { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
