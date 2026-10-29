using CargoTrack.DTO.DTOs.Cargos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Cargos
{
    public class DeliverCargoValidator : AbstractValidator<DeliverCargoDto>
    {
        public DeliverCargoValidator()
        {
            RuleFor(x => x.CargoId).NotEmpty().WithMessage("Kargo seçilmelidir.");
            RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Teslim eden personel belirtilmelidir.");

            RuleFor(x => x.DeliveryCode)
                .NotEmpty().WithMessage("Teslimat kodu girilmelidir.")
                .Matches(@"^\d{6}$").WithMessage("Teslimat kodu 6 haneli bir sayı olmalıdır.");

            RuleFor(x => x.ReceivedByName)
                .NotEmpty().WithMessage("Teslim alan kişinin adı girilmelidir.")
                .MaximumLength(100).WithMessage("Teslim alan kişinin adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Note).MaximumLength(500).WithMessage("Not en fazla 500 karakter olabilir.");
        }
    }
}