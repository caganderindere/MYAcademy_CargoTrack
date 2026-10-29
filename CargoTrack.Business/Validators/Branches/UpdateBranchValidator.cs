using CargoTrack.DTO.DTOs.BranchDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Branches
{
    public class UpdateBranchValidator : AbstractValidator<UpdateBranchDto>
    {
        public UpdateBranchValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Şube seçilmelidir.");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Şube adı boş bırakılamaz.")
                                .MaximumLength(100).WithMessage("Şube adı en fazla 100 karakter olabilir.");
            RuleFor(x => x.Address).MaximumLength(500).WithMessage("Adres en fazla 500 karakter olabilir.");
            RuleFor(x => x.PhoneNumber).Matches(@"^0?\d{10}$").WithMessage("Telefon numarası 0XXXXXXXXXX formatında olmalıdır.")
                                       .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
            RuleFor(x => x.CityId).NotEmpty().WithMessage("Şehir seçilmelidir.");
        }
    }
}
