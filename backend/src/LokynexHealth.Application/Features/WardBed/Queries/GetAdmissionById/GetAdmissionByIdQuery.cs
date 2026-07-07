using MediatR;
using LokynexHealth.Application.Features.WardBed.DTOs;

namespace LokynexHealth.Application.Features.WardBed.Queries.GetAdmissionById;

public class GetAdmissionByIdQuery : IRequest<AdmissionDto?>
{
    public Guid Id { get; set; }
}