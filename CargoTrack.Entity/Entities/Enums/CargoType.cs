using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    public enum CargoType
    {
        [Display(Name = "Dosya / Evrak")]
        Document = 1,

        [Display(Name = "Koli")]
        Package = 2,

        [Display(Name = "Kırılabilir")]
        Fragile = 3,

        [Display(Name = "Ağır Yük")]
        Heavy = 4
    }
}
