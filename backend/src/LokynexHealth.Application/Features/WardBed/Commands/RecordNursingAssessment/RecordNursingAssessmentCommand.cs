using MediatR;

namespace LokynexHealth.Application.Features.WardBed.Commands.RecordNursingAssessment;

public class RecordNursingAssessmentCommand : IRequest<Guid>
{
    public Guid AdmissionId { get; set; }
    public Guid? AssessedBy { get; set; }
    public string? VitalsJson { get; set; }
    public short? GcsScore { get; set; }
    public short? BradenScore { get; set; }
    public short? MorseFallScore { get; set; }
    public short? Nrs2002Score { get; set; }
}