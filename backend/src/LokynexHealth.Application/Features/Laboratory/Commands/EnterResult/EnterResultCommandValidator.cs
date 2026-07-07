using FluentValidation;

namespace LokynexHealth.Application.Features.Laboratory.Commands.EnterResult;

public class EnterResultCommandValidator : AbstractValidator<EnterResultCommand>
{
    public EnterResultCommandValidator()
    {
        RuleFor(x => x.OrderTestId).NotEmpty();
        RuleFor(x => x.ParameterName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ResultValue).MaximumLength(100);
        RuleFor(x => x.Unit).MaximumLength(30);
    }
}