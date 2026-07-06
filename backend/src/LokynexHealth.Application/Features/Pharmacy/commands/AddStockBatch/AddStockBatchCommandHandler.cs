using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Pharmacy.Commands.AddStockBatch;

public class AddStockBatchCommandHandler : IRequestHandler<AddStockBatchCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddStockBatchCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AddStockBatchCommand request, CancellationToken cancellationToken)
    {
        var drugExists = await _context.PharmacyDrugCatalog
            .AnyAsync(d => d.Id == request.DrugId && d.IsActive, cancellationToken);

        if (!drugExists)
        {
            throw new KeyNotFoundException($"Active drug with Id '{request.DrugId}' was not found.");
        }

        var batch = new PharmacyStockBatch
        {
            DrugId = request.DrugId,
            BatchNumber = request.BatchNumber,
            ExpiryDate = request.ExpiryDate,
            QuantityReceived = request.QuantityReceived,
            QuantityOnHand = request.QuantityReceived,
            PurchasePrice = request.PurchasePrice,
            Mrp = request.Mrp,
            SupplierName = request.SupplierName
        };

        _context.PharmacyStockBatches.Add(batch);
        await _context.SaveChangesAsync(cancellationToken);

        return batch.Id;
    }
}