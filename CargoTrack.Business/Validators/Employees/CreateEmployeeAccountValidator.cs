using CargoTrack.DTO.DTOs.EmployeeDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Employees
{
    public class CreateEmployeeAccountValidator : AbstractValidator<CreateEmployeeAccountDto>
    {
        public CreateEmployeeAccountValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Personel seçilmelidir.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.")
                                    .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email boş bırakılamaz.")
                                 .EmailAddress().WithMessage("Geçerli bir email adresi girin.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre boş bırakılamaz.");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Şifreler birbiriyle uyumlu değil");
        }
    }
}
