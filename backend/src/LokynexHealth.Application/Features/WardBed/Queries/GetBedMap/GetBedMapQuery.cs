using MediatR;
using LokynexHealth.Application.Features.WardBed.DTOs;

namespace LokynexHealth.Application.Features.WardBed.Queries.GetBedMap;

public class GetBedMapQuery : IRequest<List<WardBedMapDto>>
{
}