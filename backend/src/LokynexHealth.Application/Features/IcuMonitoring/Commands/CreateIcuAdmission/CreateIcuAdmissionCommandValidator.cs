using FluentValidation;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.CreateIcuAdmission;

public class CreateIcuAdmissionCommandValidator : AbstractValidator<CreateIcuAdmissionCommand>
{
    public CreateIcuAdmissionCommandValidator()
    {
        RuleFor(x => x.AdmissionId).NotEmpty();
        RuleFor(x => x.IcuUnitType).IsInEnum();
        RuleFor(x => x.ApacheIiScore).InclusiveBetween((short)0, (short)71).When(x => x.ApacheIiScore.HasValue);
        RuleFor(x => x.SofaScore).InclusiveBetween((short)0, (short)24).When(x => x.SofaScore.HasValue);
    }
}