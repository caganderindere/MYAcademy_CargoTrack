using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.Entity.Entities.Enums;
using FluentValidation;

namespace CargoTrack.Business.Validators.Cargos
{
    public class FailedDeliveryValidator : AbstractValidator<FailedDeliveryDto>
    {
        public FailedDeliveryValidator()
        {
            RuleFor(x => x.CargoId).NotEmpty().WithMessage("Kargo seçilmelidir.");
            RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Personel belirtilmelidir.");
            RuleFor(x => x.Reason).IsInEnum().WithMessage("Geçerli bir neden seçilmelidir.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("'Diğer' nedeni seçildiğinde açıklama yazılmalıdır.")
                .When(x => x.Reason == DeliveryExceptionReason.Other);
        }
    }
}