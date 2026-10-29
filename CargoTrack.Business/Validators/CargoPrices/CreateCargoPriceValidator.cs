using CargoTrack.DTO.DTOs.CargoPriceDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.CargoPrices
{
    public class CreateCargoPriceValidator : AbstractValidator<CreateCargoPriceDto>
    {
        private const decimal MaxDesiLimit = 1000m;

        public CreateCargoPriceValidator()
        {
            RuleFor(x => x.MinDesi).GreaterThanOrEqualTo(0).WithMessage("Minimum desi 0'dan küçük olamaz.");
            RuleFor(x => x.MaxDesi).GreaterThan(x => x.MinDesi).WithMessage("Maksimum desi, minimum desiden büyük olmalıdır.")
                                   .LessThanOrEqualTo(MaxDesiLimit).WithMessage($"Maksimum desi en fazla {MaxDesiLimit} olabilir.");
            RuleFor(x => x.DistanceZone).IsInEnum().WithMessage("Geçerli bir mesafe bölgesi seçilmelidir.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");
        }
    }
}
