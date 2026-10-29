using CargoTrack.DTO.DTOs.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.Cargos
{
    public class CargoDetailDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }
        public string? Description { get; set; }
        public decimal Weight { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public decimal Desi { get; set; }
        public CargoType CargoType { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public AdditionalService AdditionalServices { get; set; }
        public decimal Price { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public int FailedDeliveryAttempts { get; set; }
        public string? DeliveryCode { get; set; }
        public Guid SenderId { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderPhoneNumber { get; set; }
        public Guid ReceiverId { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string DeliveryAddressTitle { get; set; }
        public string DeliveryAddressDistrict { get; set; }
        public string DeliveryAddressFullAddress { get; set; }
        public string DeliveryAddressCityName { get; set; }
        public Guid OriginBranchId { get; set; }
        public string OriginBranchName { get; set; }
        public string OriginBranchCityName { get; set; }
        public Guid? OriginBranchTransferCenterId { get; set; }
        public Guid DestinationBranchId { get; set; }
        public string DestinationBranchName { get; set; }
        public string DestinationBranchCityName { get; set; }
        public Guid? DestinationBranchTransferCenterId { get; set; }
        public string? DeliveryReceivedByName { get; set; }
        public DateTime? DeliveryDeliveredDate { get; set; }
        public string? DeliveryNote { get; set; }
        public string? DeliveryEmployeeFirstName { get; set; }
        public string? DeliveryEmployeeLastName { get; set; }
        public List<ResultCargoMovementDto> Movements { get; set; } = new();
        public List<DeliveryExceptionDto> DeliveryExceptions { get; set; } = new();
        public List<CargoStatus> NextStatuses { get; set; } = new();
        public List<OptionDto> AssignableEmployees { get; set; } = new();
        public Guid? CurrentEmployeeId { get; set; }
        public bool IsDelayed { get; set; }
        public bool CanManage { get; set; }
        public bool IsReceiver { get; set; }
        public string SenderFullName => $"{SenderFirstName} {SenderLastName}";
        public string ReceiverFullName => $"{ReceiverFirstName} {ReceiverLastName}";
        public string? DeliveryEmployeeFullName => DeliveryEmployeeFirstName is null ? null : $"{DeliveryEmployeeFirstName} {DeliveryEmployeeLastName}";
    }
}
