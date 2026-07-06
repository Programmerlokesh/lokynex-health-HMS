using MediatR;
using LokynexHealth.Application.Features.Pharmacy.DTOs;

namespace LokynexHealth.Application.Features.Pharmacy.Queries.GetStockByDrug;

public class GetStockByDrugQuery : IRequest<List<StockBatchDto>>
{
    public Guid DrugId { get; set; }
}