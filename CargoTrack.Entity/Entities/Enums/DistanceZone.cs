using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    public enum DistanceZone
    {
        [Display(Name = "Şehir İçi")]
        SameCity = 1,

        [Display(Name = "Bölge İçi")]
        SameRegion = 2,

        [Display(Name = "Bölgeler Arası")]
        InterRegion = 3
    }
}
