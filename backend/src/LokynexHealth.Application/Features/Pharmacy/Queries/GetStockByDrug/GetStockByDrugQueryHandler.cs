using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Pharmacy.DTOs;

namespace LokynexHealth.Application.Features.Pharmacy.Queries.GetStockByDrug;

public class GetStockByDrugQueryHandler : IRequestHandler<GetStockByDrugQuery, List<StockBatchDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStockByDrugQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockBatchDto>> Handle(GetStockByDrugQuery request, CancellationToken cancellationToken)
    {
        return await _context.PharmacyStockBatches
            .AsNoTracking()
            .Where(b => b.DrugId == request.DrugId)
            .OrderBy(b => b.ExpiryDate) // FEFO order
            .Select(b => new StockBatchDto
            {
                Id = b.Id,
                DrugId = b.DrugId,
                BatchNumber = b.BatchNumber,
                ExpiryDate = b.ExpiryDate,
                QuantityReceived = b.QuantityReceived,
                QuantityOnHand = b.QuantityOnHand,
                PurchasePrice = b.PurchasePrice,
                Mrp = b.Mrp,
                SupplierName = b.SupplierName,
                ReceivedAt = b.ReceivedAt
            })
            .ToListAsync(cancellationToken);
    }
}