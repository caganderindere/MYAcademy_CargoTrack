using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities
{
    public class City: BaseEntity
    {
        public string Name { get; set; }

        public Region Region { get; set; }

public virtual IList<Branch> Branches { get; set; }
        public virtual IList<TransferCenter> TransferCenters { get; set; }
        public virtual IList<Address> Addresses { get; set; }
    }
}
