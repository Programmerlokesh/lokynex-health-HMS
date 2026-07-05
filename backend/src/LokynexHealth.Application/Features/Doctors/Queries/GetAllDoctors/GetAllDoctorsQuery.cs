using MediatR;
using LokynexHealth.Application.Features.Doctors.DTOs;

namespace LokynexHealth.Application.Features.Doctors.Queries.GetAllDoctors;

public class GetAllDoctorsQuery : IRequest<List<DoctorDto>>
{
    public Guid TenantId { get; set; }
}
