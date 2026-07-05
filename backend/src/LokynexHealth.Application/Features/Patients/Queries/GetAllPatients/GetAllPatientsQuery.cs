using MediatR;
using LokynexHealth.Application.Features.Patients.DTOs;

namespace LokynexHealth.Application.Features.Patients.Queries.GetAllPatients;

public class GetAllPatientsQuery : IRequest<List<PatientDto>>
{
    public Guid TenantId { get; set; }
}
