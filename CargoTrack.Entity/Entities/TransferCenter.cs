using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    public class TransferCenter: BaseEntity
    {
        public string Name { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CityId { get; set; }



        //Navigation Properties

        public virtual City City { get; set; }
        public virtual IList<Branch> Branches { get; set; }
        public virtual IList<Employee> Employees { get; set; }
        public virtual IList<CargoMovement> Movements { get; set; }
    }
}
