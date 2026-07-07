using FluentValidation;

namespace LokynexHealth.Application.Features.Laboratory.Commands.CreateLabOrder;

public class CreateLabOrderCommandValidator : AbstractValidator<CreateLabOrderCommand>
{
    public CreateLabOrderCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.TestIds).NotEmpty().WithMessage("At least one test must be selected.");
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.SchemeTag).MaximumLength(20);
    }
}