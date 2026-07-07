using FluentValidation;

namespace LokynexHealth.Application.Features.WardBed.Commands.TransferBed;

public class TransferBedCommandValidator : AbstractValidator<TransferBedCommand>
{
    public TransferBedCommandValidator()
    {
        RuleFor(x => x.AdmissionId).NotEmpty();
        RuleFor(x => x.ToBedId).NotEmpty();
    }
}