using MediatR;
using LokynexHealth.Application.Features.Pharmacy.DTOs;

namespace LokynexHealth.Application.Features.Pharmacy.Queries.GetAllDrugs;

public class GetAllDrugsQuery : IRequest<List<DrugDto>>
{
    public bool IncludeInactive { get; set; } = false;
}