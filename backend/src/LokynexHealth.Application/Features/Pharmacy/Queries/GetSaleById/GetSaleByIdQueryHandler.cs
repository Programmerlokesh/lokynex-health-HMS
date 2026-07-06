using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Pharmacy.DTOs;

namespace LokynexHealth.Application.Features.Pharmacy.Queries.GetSaleById;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto?>
{
    private readonly IApplicationDbContext _context;

    public GetSaleByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SaleDto?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _context.PharmacySales
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (sale is null)
        {
            return null;
        }

        var items = await _context.PharmacySaleItems
            .AsNoTracking()
            .Where(i => i.SaleId == request.Id)
            .Select(i => new SaleItemDto
            {
                Id = i.Id,
                DrugId = i.DrugId,
                BatchId = i.BatchId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                GstAmount = i.GstAmount,
                LineTotal = i.LineTotal
            })
            .ToListAsync(cancellationToken);

        return new SaleDto
        {
            Id = sale.Id,
            InvoiceNumber = sale.InvoiceNumber,
            PatientId = sale.PatientId,
            PrescriptionId = sale.PrescriptionId,
            Subtotal = sale.Subtotal,
            CgstAmount = sale.CgstAmount,
            SgstAmount = sale.SgstAmount,
            TotalAmount = sale.TotalAmount,
            PaymentStatus = sale.PaymentStatus.ToString(),
            PaymentMethod = sale.PaymentMethod?.ToString(),
            EwayBillTriggered = sale.EwayBillTriggered,
            EwayBillNumber = sale.EwayBillNumber,
            SoldAt = sale.SoldAt,
            Items = items
        };
    }
}