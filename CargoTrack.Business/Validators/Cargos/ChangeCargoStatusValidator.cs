using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.Entity.Entities.Enums;
using FluentValidation;

namespace CargoTrack.Business.Validators.Cargos
{
    public class ChangeCargoStatusValidator : AbstractValidator<ChangeCargoStatusDto>
    {
        public ChangeCargoStatusValidator()
        {
            RuleFor(x => x.CargoId).NotEmpty().WithMessage("Kargo seçilmelidir.");
            RuleFor(x => x.NewStatus).IsInEnum().WithMessage("Geçerli bir durum seçilmelidir.");
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

            RuleFor(x => x.TransferCenterId)
                .NotEmpty().WithMessage("Transfer merkezi seçilmelidir.")
                .When(x => x.NewStatus == CargoStatus.InTransferCenter);

            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("Şube seçilmelidir.")
                .When(x => x.NewStatus == CargoStatus.AtOriginBranch
                        || x.NewStatus == CargoStatus.AtDestinationBranch
                        || x.NewStatus == CargoStatus.OutForDelivery);
        }
    }
}