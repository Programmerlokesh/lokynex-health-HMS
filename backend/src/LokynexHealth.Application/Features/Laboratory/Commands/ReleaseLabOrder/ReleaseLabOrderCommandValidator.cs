using FluentValidation;

namespace LokynexHealth.Application.Features.Laboratory.Commands.ReleaseLabOrder;

public class ReleaseLabOrderCommandValidator : AbstractValidator<ReleaseLabOrderCommand>
{
    public ReleaseLabOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}