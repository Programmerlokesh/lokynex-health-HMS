using MediatR;
using LokynexHealth.Application.Features.Icu.DTOs;

namespace LokynexHealth.Application.Features.Icu.Queries.GetIcuAdmissionById;

public class GetIcuAdmissionByIdQuery : IRequest<IcuAdmissionDto?>
{
    public Guid Id { get; set; }
}