using MediatR;

namespace LokynexHealth.Application.Features.WardBed.Commands.TransferBed;

public class TransferBedCommand : IRequest<Guid>
{
    public Guid AdmissionId { get; set; }
    public Guid ToBedId { get; set; }
    public string? IsbarIdentify { get; set; }
    public string? IsbarSituation { get; set; }
    public string? IsbarBackground { get; set; }
    public string? IsbarAssessment { get; set; }
    public string? IsbarRecommendation { get; set; }
    public Guid? TransferredBy { get; set; }
}