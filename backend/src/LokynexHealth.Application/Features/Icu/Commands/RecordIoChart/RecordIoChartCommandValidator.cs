using FluentValidation;

namespace LokynexHealth.Application.Features.Icu.Commands.RecordIoChart;

public class RecordIoChartCommandValidator : AbstractValidator<RecordIoChartCommand>
{
    public RecordIoChartCommandValidator()
    {
        RuleFor(x => x.IcuAdmissionId).NotEmpty();
        RuleFor(x => x.IntakeMl).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OutputMl).GreaterThanOrEqualTo(0);
    }
}