namespace LokynexHealth.Application.Features.Icu.DTOs;

public class IcuIoChartDto
{
    public Guid Id { get; set; }
    public Guid IcuAdmissionId { get; set; }
    public DateOnly ChartDate { get; set; }
    public decimal IntakeMl { get; set; }
    public decimal OutputMl { get; set; }
    public decimal NetBalanceMl { get; set; }
    public Guid? RecordedBy { get; set; }
}
