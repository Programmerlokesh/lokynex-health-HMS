using FluentValidation;

namespace LokynexHealth.Application.Features.WardBed.Commands.CreateBed;

public class CreateBedCommandValidator : AbstractValidator<CreateBedCommand>
{
    public CreateBedCommandValidator()
    {
        RuleFor(x => x.WardId).NotEmpty();
        RuleFor(x => x.BedNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.BedCategory).MaximumLength(30);
        RuleFor(x => x.DailyRate).GreaterThanOrEqualTo(0).When(x => x.DailyRate.HasValue);
    }
}