using FluentValidation;

namespace LokynexHealth.Application.Features.DoctorOpd.Commands.CreateOpdVisit;

public class CreateOpdVisitCommandValidator : AbstractValidator<CreateOpdVisitCommand>
{
    public CreateOpdVisitCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.SchemeTag).MaximumLength(30);
        RuleFor(x => x.ChiefComplaint).MaximumLength(500);
    }
}
