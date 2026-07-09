using FluentValidation;

namespace LokynexHealth.Application.Features.Patients.Commands.DeletePatient;

public class DeletePatientCommandValidator : AbstractValidator<DeletePatientCommand>
{
    public DeletePatientCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}