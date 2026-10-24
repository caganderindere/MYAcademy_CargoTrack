using Microsoft.AspNetCore.Identity;

namespace CargoTrack.Entity.Entities
{
    public class AppUser: IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }


        //Navigation Properties

        public virtual Customer? Customer { get; set; }

        public virtual Employee? Employee { get; set; }

    }
}
