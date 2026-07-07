using MediatR;

namespace LokynexHealth.Application.Features.Laboratory.Commands.EnterResult;

public class EnterResultCommand : IRequest<Guid>
{
    public Guid OrderTestId { get; set; }
    public Guid? SampleId { get; set; }
    public string ParameterName { get; set; } = string.Empty;
    public string? ResultValue { get; set; }
    public string? Unit { get; set; }
    public string? ReferenceRange { get; set; }
    public bool IsCritical { get; set; }
    public bool IsAbnormal { get; set; }
    public Guid? EnteredBy { get; set; }
}