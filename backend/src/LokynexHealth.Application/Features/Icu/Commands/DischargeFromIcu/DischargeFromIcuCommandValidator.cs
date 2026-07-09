using FluentValidation;

namespace LokynexHealth.Application.Features.Icu.Commands.DischargeFromIcu;

public class DischargeFromIcuCommandValidator : AbstractValidator<DischargeFromIcuCommand>
{
    public DischargeFromIcuCommandValidator()
    {
        RuleFor(x => x.IcuAdmissionId).NotEmpty();
    }
}