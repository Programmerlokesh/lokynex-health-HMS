using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Pharmacy.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private const decimal EWayBillThreshold = 50000m;

    private readonly IApplicationDbContext _context;

    public CreateSaleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var saleItems = new List<PharmacySaleItem>();

        foreach (var line in request.Items)
        {
            var drug = await _context.PharmacyDrugCatalog
                .FirstOrDefaultAsync(d => d.Id == line.DrugId && d.IsActive, cancellationToken);

            if (drug is null)
            {
                throw new KeyNotFoundException($"Active drug with Id '{line.DrugId}' was not found.");
            }

            var remainingQty = line.Quantity;

            // Candidate batches: either the one explicitly picked, or FEFO order (earliest expiry first)
            var candidateBatches = line.BatchId.HasValue
                ? await _context.PharmacyStockBatches
                    .Where(b => b.Id == line.BatchId.Value && b.DrugId == line.DrugId)
                    .ToListAsync(cancellationToken)
                : await _context.PharmacyStockBatches
                    .Where(b => b.DrugId == line.DrugId && b.QuantityOnHand > 0 && b.ExpiryDate >= today)
                    .OrderBy(b => b.ExpiryDate)
                    .ToListAsync(cancellationToken);

            foreach (var batch in candidateBatches)
            {
                if (remainingQty <= 0)
                {
                    break;
                }

                var qtyFromThisBatch = Math.Min(remainingQty, batch.QuantityOnHand);
                if (qtyFromThisBatch <= 0)
                {
                    continue;
                }

                var unitPrice = batch.Mrp ?? 0m;
                var gstAmount = qtyFromThisBatch * unitPrice * drug.GstRatePct / 100m;
                var lineTotal = (qtyFromThisBatch * unitPrice) + gstAmount;

                batch.QuantityOnHand -= qtyFromThisBatch;

                saleItems.Add(new PharmacySaleItem
                {
                    DrugId = drug.Id,
                    BatchId = batch.Id,
                    Quantity = qtyFromThisBatch,
                    UnitPrice = unitPrice,
                    GstAmount = gstAmount,
                    LineTotal = lineTotal
                });

                remainingQty -= qtyFromThisBatch;
            }

            if (remainingQty > 0)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for drug '{drug.DrugName}'. Short by {remainingQty} {drug.UnitOfMeasure}.");
            }
        }

        var subtotal = saleItems.Sum(i => i.Quantity * i.UnitPrice);
        var totalGst = saleItems.Sum(i => i.GstAmount);
        var cgst = totalGst / 2m;
        var sgst = totalGst / 2m;
        var totalAmount = subtotal + cgst + sgst;

        var sale = new PharmacySale
        {
            InvoiceNumber = $"RX-INV-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            PatientId = request.PatientId,
            PrescriptionId = request.PrescriptionId,
            Subtotal = subtotal,
            CgstAmount = cgst,
            SgstAmount = sgst,
            TotalAmount = totalAmount,
            PaymentMethod = request.PaymentMethod,
            EwayBillTriggered = totalAmount > EWayBillThreshold
        };

        _context.PharmacySales.Add(sale);

        foreach (var item in saleItems)
        {
            item.SaleId = sale.Id;
            _context.PharmacySaleItems.Add(item);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }
}