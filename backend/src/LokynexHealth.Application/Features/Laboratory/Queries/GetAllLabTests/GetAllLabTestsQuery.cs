using MediatR;
using LokynexHealth.Application.Features.Laboratory.DTOs;

namespace LokynexHealth.Application.Features.Laboratory.Queries.GetAllLabTests;

public class GetAllLabTestsQuery : IRequest<List<LabTestDto>>
{
    public bool IncludeInactive { get; set; } = false;
}