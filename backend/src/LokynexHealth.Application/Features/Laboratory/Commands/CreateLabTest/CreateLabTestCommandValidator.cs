using FluentValidation;

namespace LokynexHealth.Application.Features.Laboratory.Commands.CreateLabTest;

public class CreateLabTestCommandValidator : AbstractValidator<CreateLabTestCommand>
{
    public CreateLabTestCommandValidator()
    {
        RuleFor(x => x.TestCode).NotEmpty().MaximumLength(30);
        RuleFor(x => x.TestName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SpecimenType).MaximumLength(50);
        RuleFor(x => x.StandardPrice).GreaterThanOrEqualTo(0);
    }
}