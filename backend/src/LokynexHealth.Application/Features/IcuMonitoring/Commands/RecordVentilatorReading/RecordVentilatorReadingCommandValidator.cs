using FluentValidation;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.RecordVentilatorReading;

public class RecordVentilatorReadingCommandValidator : AbstractValidator<RecordVentilatorReadingCommand>
{
    public RecordVentilatorReadingCommandValidator()
    {
        RuleFor(x => x.IcuAdmissionId).NotEmpty();
        RuleFor(x => x.Mode).MaximumLength(30);
        RuleFor(x => x.Fio2Pct).InclusiveBetween((short)21, (short)100).When(x => x.Fio2Pct.HasValue);
    }
}