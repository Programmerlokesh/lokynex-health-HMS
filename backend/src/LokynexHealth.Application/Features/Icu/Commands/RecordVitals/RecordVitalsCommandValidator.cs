using FluentValidation;

namespace LokynexHealth.Application.Features.Icu.Commands.RecordVitals;

public class RecordVitalsCommandValidator : AbstractValidator<RecordVitalsCommand>
{
    public RecordVitalsCommandValidator()
    {
        RuleFor(x => x.IcuAdmissionId).NotEmpty();
        RuleFor(x => x.Spo2Pct).InclusiveBetween((short)0, (short)100).When(x => x.Spo2Pct.HasValue);
        RuleFor(x => x.HeartRate).InclusiveBetween((short)0, (short)300).When(x => x.HeartRate.HasValue);
    }
}