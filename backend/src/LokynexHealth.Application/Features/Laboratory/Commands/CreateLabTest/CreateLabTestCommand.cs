using MediatR;

namespace LokynexHealth.Application.Features.Laboratory.Commands.CreateLabTest;

public class CreateLabTestCommand : IRequest<Guid>
{
    public string TestCode { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string? LoincCode { get; set; }
    public string? SpecimenType { get; set; }
    public string? NablPanel { get; set; }
    public decimal StandardPrice { get; set; }
    public decimal? CghsPrice { get; set; }
    public decimal? TatHoursStd { get; set; }
}