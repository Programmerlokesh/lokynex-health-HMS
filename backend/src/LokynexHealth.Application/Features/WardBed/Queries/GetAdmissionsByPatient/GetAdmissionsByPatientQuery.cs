using MediatR;
using LokynexHealth.Application.Features.WardBed.DTOs;

namespace LokynexHealth.Application.Features.WardBed.Queries.GetAdmissionsByPatient;

public class GetAdmissionsByPatientQuery : IRequest<List<AdmissionDto>>
{
    public Guid PatientId { get; set; }
}