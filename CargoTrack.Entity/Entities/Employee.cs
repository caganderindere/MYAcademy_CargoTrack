using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities
{
    public class Employee: BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public EmployeePosition Position { get; set; }

        public Guid? BranchId { get; set; }
        public Guid? TransferCenterId { get; set; }

        public Guid? AppUserId { get; set; }

        public virtual Branch? Branch { get; set; }
        public virtual TransferCenter? TransferCenter { get; set; }
        public virtual AppUser? AppUser { get; set; }
        public virtual IList<CargoMovement> Movements { get; set; }
        public virtual IList<Delivery> Deliveries { get; set; }
        public virtual IList<DeliveryException> DeliveryExceptions { get; set; }
    }
}
