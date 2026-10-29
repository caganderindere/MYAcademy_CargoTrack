using CargoTrack.DTO.DTOs.AddressDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Addresses
{
    public class CreateAddressValidator : AbstractValidator<CreateAddressDto>
    {
        public CreateAddressValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Adres başlığı boş bırakılamaz.")
                                 .MaximumLength(50).WithMessage("Adres başlığı en fazla 50 karakter olabilir.");
            RuleFor(x => x.District).NotEmpty().WithMessage("İlçe boş bırakılamaz.")
                                    .MaximumLength(50).WithMessage("İlçe en fazla 50 karakter olabilir.");
            RuleFor(x => x.FullAddress).NotEmpty().WithMessage("Açık adres boş bırakılamaz.")
                                       .MaximumLength(500).WithMessage("Açık adres en fazla 500 karakter olabilir.");
            RuleFor(x => x.CityId).NotEmpty().WithMessage("Şehir seçilmelidir.");
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Müşteri seçilmelidir.");
        }
    }
}
