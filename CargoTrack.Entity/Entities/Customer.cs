using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    public class Customer: BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }

        public Guid? AppUserId { get; set; }



        //Navigation Properties

        public virtual AppUser? AppUser { get; set; }
        public virtual IList<Address> Addresses { get; set; }
        public virtual IList<Cargo> SentCargos { get; set; }
        public virtual IList<Cargo> ReceivedCargos { get; set; }
    }
}
