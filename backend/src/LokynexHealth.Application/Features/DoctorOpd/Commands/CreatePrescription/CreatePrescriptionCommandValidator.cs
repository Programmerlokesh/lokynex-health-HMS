using FluentValidation;

namespace LokynexHealth.Application.Features.DoctorOpd.Commands.CreatePrescription;

public class CreatePrescriptionCommandValidator : AbstractValidator<CreatePrescriptionCommand>
{
    public CreatePrescriptionCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one drug item is required.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.DrugName)
                .NotEmpty().WithMessage("Drug name is required.")
                .MaximumLength(200);

            item.RuleFor(i => i.Dosage).MaximumLength(100);
            item.RuleFor(i => i.Frequency).MaximumLength(100);

            item.RuleFor(i => i.DurationDays)
                .GreaterThan((short)0).When(i => i.DurationDays.HasValue)
                .WithMessage("Duration must be greater than zero when specified.");
        });
    }
}