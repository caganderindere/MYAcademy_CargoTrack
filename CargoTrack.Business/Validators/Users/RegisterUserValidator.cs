using CargoTrack.DTO.DTOs.UserDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Users
{
    public class RegisterUserValidator: AbstractValidator<RegisterUserDto>
    {

        public RegisterUserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad bilgisi boş bırakılamaz.")
                                     .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad bilgisi boş bırakılamaz.")
                                    .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
                                       .Matches(@"^0?5\d{9}$").WithMessage("Telefon numarası 05XXXXXXXXX formatında olmalıdır.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre boş bırakılamaz.");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Şifre tekrarı boş bırakılamaz.")
                                           .Equal(x => x.Password).WithMessage("Şifreler birbiriyle uyumlu değil");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email boş bırakılamaz.")
                                 .EmailAddress().WithMessage("Geçerli bir email adresi girin.")
                                 .MaximumLength(100).WithMessage("Email en fazla 100 karakter olabilir.");
        }

    }
}
