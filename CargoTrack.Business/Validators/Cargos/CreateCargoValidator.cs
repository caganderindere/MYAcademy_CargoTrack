using CargoTrack.DTO.DTOs.Cargos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Cargos
{
    public class CreateCargoValidator : AbstractValidator<CreateCargoDto>
    {
        public CreateCargoValidator()
        {
            RuleFor(x => x.SenderId).NotEmpty().WithMessage("Gönderici seçilmelidir.");
            RuleFor(x => x.ReceiverId).NotEmpty().WithMessage("Alıcı seçilmelidir.");
            RuleFor(x => x.DeliveryAddressId).NotEmpty().WithMessage("Teslimat adresi seçilmelidir.");
            RuleFor(x => x.OriginBranchId).NotEmpty().WithMessage("Çıkış şubesi seçilmelidir.");
            RuleFor(x => x.DestinationBranchId).NotEmpty().WithMessage("Varış şubesi seçilmelidir.");

            RuleFor(x => x.Weight).GreaterThan(0).WithMessage("Ağırlık 0'dan büyük olmalıdır.")
                                  .LessThanOrEqualTo(50).WithMessage("Ağırlık en fazla 50 kg olabilir.");

            RuleFor(x => x.Width).GreaterThan(0).WithMessage("En 0'dan büyük olmalıdır.")
                                 .LessThanOrEqualTo(200).WithMessage("En en fazla 200 cm olabilir.");

            RuleFor(x => x.Length).GreaterThan(0).WithMessage("Boy 0'dan büyük olmalıdır.")
                                  .LessThanOrEqualTo(200).WithMessage("Boy en fazla 200 cm olabilir.");

            RuleFor(x => x.Height).GreaterThan(0).WithMessage("Yükseklik 0'dan büyük olmalıdır.")
                                  .LessThanOrEqualTo(200).WithMessage("Yükseklik en fazla 200 cm olabilir.");

            RuleFor(x => x.CargoType).IsInEnum().WithMessage("Geçerli bir kargo tipi seçilmelidir.");
            RuleFor(x => x.DeliveryType).IsInEnum().WithMessage("Geçerli bir teslimat tipi seçilmelidir.");
            RuleFor(x => x.AdditionalServices).IsInEnum().WithMessage("Geçerli ek hizmetler seçilmelidir.");

            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");
        }
    }
}