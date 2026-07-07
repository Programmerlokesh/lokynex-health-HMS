using FluentValidation;

namespace LokynexHealth.Application.Features.Laboratory.Commands.CollectSample;

public class CollectSampleCommandValidator : AbstractValidator<CollectSampleCommand>
{
    public CollectSampleCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.SampleBarcode).NotEmpty().MaximumLength(50);
    }
}