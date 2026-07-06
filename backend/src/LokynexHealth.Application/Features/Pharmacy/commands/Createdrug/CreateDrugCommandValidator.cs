using FluentValidation;

namespace LokynexHealth.Application.Features.Pharmacy.Commands.CreateDrug;

public class CreateDrugCommandValidator : AbstractValidator<CreateDrugCommand>
{
    public CreateDrugCommandValidator()
    {
        RuleFor(x => x.DrugName)
            .NotEmpty().WithMessage("Drug name is required.")
            .MaximumLength(200);

        RuleFor(x => x.GenericName).MaximumLength(200);
        RuleFor(x => x.HsnCode).MaximumLength(15);
        RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(20);

        RuleFor(x => x.GstRatePct)
            .InclusiveBetween(0, 100).WithMessage("GST rate must be between 0 and 100.");

        RuleFor(x => x.ScheduleFlag).IsInEnum();
    }
}