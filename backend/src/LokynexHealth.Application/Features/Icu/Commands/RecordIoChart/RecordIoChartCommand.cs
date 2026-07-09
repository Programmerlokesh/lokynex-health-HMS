using MediatR;

namespace LokynexHealth.Application.Features.Icu.Commands.RecordIoChart;

public class RecordIoChartCommand : IRequest<Guid>
{
    public Guid IcuAdmissionId { get; set; }
    public DateOnly? ChartDate { get; set; }
    public decimal IntakeMl { get; set; }
    public decimal OutputMl { get; set; }
    public Guid? RecordedBy { get; set; }
}