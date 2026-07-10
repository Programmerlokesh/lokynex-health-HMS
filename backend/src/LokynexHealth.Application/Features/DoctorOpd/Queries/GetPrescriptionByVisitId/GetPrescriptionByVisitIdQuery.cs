using MediatR;
using LokynexHealth.Application.Features.DoctorOpd.DTOs;

namespace LokynexHealth.Application.Features.DoctorOpd.Queries.GetPrescriptionByVisitId;

public class GetPrescriptionByVisitIdQuery : IRequest<PrescriptionDto?>
{
    public Guid VisitId { get; set; }
}