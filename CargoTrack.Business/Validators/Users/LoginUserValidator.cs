using CargoTrack.DTO.DTOs.UserDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Users
{
    public class LoginUserValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre boş bırakılamaz.");
        }
    }
}
