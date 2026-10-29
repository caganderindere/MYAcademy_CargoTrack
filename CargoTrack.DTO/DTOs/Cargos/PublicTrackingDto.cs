using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.Cargos
{
    public class PublicTrackingDto
    {
        public string TrackCode { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public CargoType CargoType { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public string SenderMaskedName { get; set; }
        public string ReceiverMaskedName { get; set; }
        public string OriginCityName { get; set; }
        public string OriginBranchName { get; set; }
        public string DestinationCityName { get; set; }
        public string DestinationBranchName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public bool IsDelayed { get; set; }
        public int FailedDeliveryAttempts { get; set; }
        public PublicMovementDto? LastMovement { get; set; }
        public List<PublicMovementDto> Movements { get; set; } = new();
    }

    public class PublicMovementDto
    {
        public CargoStatus ToStatus { get; set; }
        public DateTime MovementDate { get; set; }
        public string? LocationName { get; set; }
        public string? Description { get; set; }
    }
}
