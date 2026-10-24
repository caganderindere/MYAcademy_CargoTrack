using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    public enum DeliveryExceptionReason
    {
        [Display(Name = "Adres Hatalı")]
        WrongAddress = 1,

        [Display(Name = "Alıcı Adreste Yok")]
        RecipientNotAvailable = 2,

        [Display(Name = "Hasarlı Kargo")]
        Damaged = 3,

        [Display(Name = "Alıcı Reddetti")]
        Refused = 4,

        [Display(Name = "Diğer")]
        Other = 99
    }
}
