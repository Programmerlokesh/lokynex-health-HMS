using FluentValidation;

namespace LokynexHealth.Application.Features.WardBed.Commands.CreateWard;

public class CreateWardCommandValidator : AbstractValidator<CreateWardCommand>
{
    public CreateWardCommandValidator()
    {
        RuleFor(x => x.WardName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.WardType).MaximumLength(50);
        RuleFor(x => x.Floor).MaximumLength(20);
    }
}