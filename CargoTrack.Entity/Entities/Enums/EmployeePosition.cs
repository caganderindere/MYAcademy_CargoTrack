using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    public enum EmployeePosition
    {
        [Display(Name = "Kurye")]
        Courier = 1,

        [Display(Name = "Şube Personeli")]
        BranchStaff = 2,

        [Display(Name = "Transfer Merkezi Personeli")]
        TransferCenterStaff = 3,

        [Display(Name = "Şube Yöneticisi")]
        BranchManager = 4
    }
}
