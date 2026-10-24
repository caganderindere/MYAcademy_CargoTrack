using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    public class Branch: BaseEntity
    {
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CityId { get; set; }

        public Guid? TransferCenterId { get; set; }



        //Navigation Properties

        public virtual City City { get; set; }
        public virtual TransferCenter? TransferCenter { get; set; }
        public virtual IList<Employee> Employees { get; set; }
        public virtual IList<Cargo> OriginCargos { get; set; }
        public virtual IList<Cargo> DestinationCargos { get; set; }
        public virtual IList<CargoMovement> Movements { get; set; }
    }
}
