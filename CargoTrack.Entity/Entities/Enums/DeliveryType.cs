using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    public enum DeliveryType
    {
        [Display(Name = "Standart")]
        Standard = 1,

        [Display(Name = "Hızlı")]
        Express = 2,

        [Display(Name = "Aynı Gün")]
        SameDay = 3
    }
}
