using CargoTrack.DTO.DTOs.EmployeeDtos;
using CargoTrack.Entity.Entities.Enums;
using FluentValidation;

namespace CargoTrack.Business.Validators.Employees
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad boş bırakılamaz.")
                                     .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad boş bırakılamaz.")
                                    .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
                                       .Matches(@"^0?5\d{9}$").WithMessage("Telefon numarası 05XXXXXXXXX formatında olmalıdır.");
            RuleFor(x => x.Position).IsInEnum().WithMessage("Geçerli bir pozisyon seçilmelidir.");

            When(x => x.Position == EmployeePosition.TransferCenterStaff, () =>
            {
                RuleFor(x => x.TransferCenterId).NotEmpty().WithMessage("Transfer merkezi personeli için transfer merkezi seçilmelidir.");
                RuleFor(x => x.BranchId).Null().WithMessage("Transfer merkezi personeli bir şubeye bağlanamaz.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.BranchId).NotEmpty().WithMessage("Bu pozisyon için şube seçilmelidir.");
                RuleFor(x => x.TransferCenterId).Null().WithMessage("Şube personeli bir transfer merkezine bağlanamaz.");
            });
        }
    }
}