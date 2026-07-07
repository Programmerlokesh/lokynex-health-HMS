using MediatR;
using LokynexHealth.Application.Features.WardBed.DTOs;

namespace LokynexHealth.Application.Features.WardBed.Queries.GetNursingAssessmentsByAdmission;

public class GetNursingAssessmentsByAdmissionQuery : IRequest<List<NursingAssessmentDto>>
{
    public Guid AdmissionId { get; set; }
}