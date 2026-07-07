using FluentValidation;

namespace LokynexHealth.Application.Features.WardBed.Commands.AdmitPatient;

public class AdmitPatientCommandValidator : AbstractValidator<AdmitPatientCommand>
{
    public AdmitPatientCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.BedId).NotEmpty();
        RuleFor(x => x.PmjayPackageCode).MaximumLength(30);
    }
}