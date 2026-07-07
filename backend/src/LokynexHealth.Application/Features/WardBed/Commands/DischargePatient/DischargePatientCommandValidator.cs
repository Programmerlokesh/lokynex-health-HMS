using FluentValidation;

namespace LokynexHealth.Application.Features.WardBed.Commands.DischargePatient;

public class DischargePatientCommandValidator : AbstractValidator<DischargePatientCommand>
{
    public DischargePatientCommandValidator()
    {
        RuleFor(x => x.AdmissionId).NotEmpty();
    }
}