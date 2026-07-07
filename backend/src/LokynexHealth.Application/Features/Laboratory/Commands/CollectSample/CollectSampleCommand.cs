using MediatR;

namespace LokynexHealth.Application.Features.Laboratory.Commands.CollectSample;

public class CollectSampleCommand : IRequest<Guid>
{
    public Guid OrderId { get; set; }
    public string SampleBarcode { get; set; } = string.Empty;
    public Guid? CollectedBy { get; set; }
}