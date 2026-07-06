using FluentValidation;

namespace LokynexHealth.Application.Features.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Doctor's full name is required.")
            .MaximumLength(200);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\d{10}$").WithMessage("Phone number must be exactly 10 digits.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email address is not valid.");

        RuleFor(x => x.RegistrationNumber)
            .NotEmpty().WithMessage("NMC registration number is required.");
    }
}
