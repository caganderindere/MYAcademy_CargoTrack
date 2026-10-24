using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    public enum Region
    {
        [Display(Name = "Marmara")]
        Marmara = 1,

        [Display(Name = "Ege")]
        Aegean = 2,

        [Display(Name = "Akdeniz")]
        Mediterranean = 3,

        [Display(Name = "İç Anadolu")]
        CentralAnatolia = 4,

        [Display(Name = "Karadeniz")]
        BlackSea = 5,

        [Display(Name = "Doğu Anadolu")]
        EasternAnatolia = 6,

        [Display(Name = "Güneydoğu Anadolu")]
        SoutheasternAnatolia = 7
    }
}
