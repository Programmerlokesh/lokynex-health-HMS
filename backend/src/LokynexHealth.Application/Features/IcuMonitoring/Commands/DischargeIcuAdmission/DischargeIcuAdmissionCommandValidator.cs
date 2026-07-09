using FluentValidation;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.DischargeIcuAdmission;

public class DischargeIcuAdmissionCommandValidator : AbstractValidator<DischargeIcuAdmissionCommand>
{
    public DischargeIcuAdmissionCommandValidator()
    {
        RuleFor(x => x.IcuAdmissionId).NotEmpty();
    }
}