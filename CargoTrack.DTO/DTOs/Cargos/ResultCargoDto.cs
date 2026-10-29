using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.Cargos
{
    public class ResultCargoDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public CargoType CargoType { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public decimal Desi { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public Guid SenderId { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public Guid ReceiverId { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public Guid OriginBranchId { get; set; }
        public string OriginBranchName { get; set; }
        public Guid DestinationBranchId { get; set; }
        public string DestinationBranchName { get; set; }
    }
}
