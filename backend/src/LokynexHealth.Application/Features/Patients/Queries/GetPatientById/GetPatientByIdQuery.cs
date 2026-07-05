using MediatR;
using LokynexHealth.Application.Features.Patients.DTOs;

namespace LokynexHealth.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQuery : IRequest<PatientDto?>
{
    public Guid Id { get; set; }
}
