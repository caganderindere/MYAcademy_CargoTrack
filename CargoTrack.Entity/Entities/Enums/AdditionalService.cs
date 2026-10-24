using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    [Flags]
    public enum AdditionalService
    {
        [Display(Name = "Yok")]
        None = 0,

        [Display(Name = "Sigorta")]
        Insurance = 1,

        [Display(Name = "Kapıda Ödeme")]
        CashOnDelivery = 2,

        [Display(Name = "SMS Bildirimi")]
        SmsNotification = 4,

        [Display(Name = "Hafta Sonu Teslimat")]
        WeekendDelivery = 8
    }
}
