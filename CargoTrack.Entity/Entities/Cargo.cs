using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities;

public class Cargo: BaseEntity
{
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
    public DateTime EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public int FailedDeliveryAttempts { get; set; }

    public string? DeliveryCode { get; set; }
    public DateTime? DeliveryCodeGeneratedDate { get; set; }

    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public Guid OriginBranchId { get; set; }
    public Guid DestinationBranchId { get; set; }



    //Navigation Properties
    public virtual Customer Sender { get; set; }
    public virtual Customer Receiver { get; set; }
    public virtual Address DeliveryAddress { get; set; }
    public virtual Branch OriginBranch { get; set; }
    public virtual Branch DestinationBranch { get; set; }
    public virtual Delivery? Delivery { get; set; }
    public virtual IList<CargoMovement> Movements { get; set; }
    public virtual IList<DeliveryException> DeliveryExceptions { get; set; }

}
