using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.Cargos
{
    public class CreateCargoDto
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid DeliveryAddressId { get; set; }
        public Guid OriginBranchId { get; set; }
        public Guid DestinationBranchId { get; set; }

        public decimal Weight { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }

        public CargoType CargoType { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public AdditionalService AdditionalServices { get; set; }

        public string? Description { get; set; }
    }
}