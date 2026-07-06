using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Pharmacy.DTOs;

namespace LokynexHealth.Application.Features.Pharmacy.Queries.GetAllSalesByPatient;

public class GetAllSalesByPatientQueryHandler : IRequestHandler<GetAllSalesByPatientQuery, List<SaleDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSalesByPatientQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SaleDto>> Handle(GetAllSalesByPatientQuery request, CancellationToken cancellationToken)
    {
        var sales = await _context.PharmacySales
            .AsNoTracking()
            .Where(s => s.PatientId == request.PatientId)
            .ToListAsync(cancellationToken);

        var saleIds = sales.Select(s => s.Id).ToList();

        var itemsBySale = await _context.PharmacySaleItems
            .AsNoTracking()
            .Where(i => saleIds.Contains(i.SaleId))
            .ToListAsync(cancellationToken);

        return sales.Select(sale => new SaleDto
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
            Items = itemsBySale
                .Where(i => i.SaleId == sale.Id)
                .Select(i => new SaleItemDto
                {
                    Id = i.Id,
                    DrugId = i.DrugId,
                    BatchId = i.BatchId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    GstAmount = i.GstAmount,
                    LineTotal = i.LineTotal
                }).ToList()
        }).ToList();
    }
}