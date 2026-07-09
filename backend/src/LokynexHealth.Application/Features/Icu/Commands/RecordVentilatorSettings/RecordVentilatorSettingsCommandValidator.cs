using FluentValidation;

namespace LokynexHealth.Application.Features.Icu.Commands.RecordVentilatorSettings;

public class RecordVentilatorSettingsCommandValidator : AbstractValidator<RecordVentilatorSettingsCommand>
{
    public RecordVentilatorSettingsCommandValidator()
    {
        RuleFor(x => x.IcuAdmissionId).NotEmpty();
        RuleFor(x => x.Mode).MaximumLength(30);
        RuleFor(x => x.Fio2Pct).InclusiveBetween((short)21, (short)100).When(x => x.Fio2Pct.HasValue);
    }
}