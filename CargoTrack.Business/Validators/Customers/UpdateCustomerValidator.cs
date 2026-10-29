using CargoTrack.DTO.DTOs.CustomerDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Customers
{
    public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDto>
    {
        public UpdateCustomerValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Müşteri seçilmelidir.");

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad boş bırakılamaz.")
                                     .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad boş bırakılamaz.")
                                    .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
                                       .Matches(@"^0?5\d{9}$").WithMessage("Telefon numarası 05XXXXXXXXX formatında olmalıdır.");

            When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
            {
                RuleFor(x => x.Email).EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                                     .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir.");
            });
        }
    }
}
