using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Laboratory.Commands.GenerateLabInvoice;

public class GenerateLabInvoiceCommandHandler : IRequestHandler<GenerateLabInvoiceCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public GenerateLabInvoiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(GenerateLabInvoiceCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.LabOrders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException($"Lab order with Id '{request.OrderId}' was not found.");
        }

        if (order.Status != LabOrderStatus.Released)
        {
            throw new InvalidOperationException("Lab order must be released before it can be billed.");
        }

        var alreadyInvoiced = await _context.BillingInvoiceItems
            .AnyAsync(i => i.SourceModule == "Laboratory" && i.SourceRecordId != null &&
                           _context.LabOrderTests.Any(ot => ot.Id == i.SourceRecordId && ot.OrderId == order.Id),
                      cancellationToken);

        if (alreadyInvoiced)
        {
            throw new InvalidOperationException("This lab order has already been invoiced.");
        }

        var orderTests = await (
            from ot in _context.LabOrderTests
            join t in _context.LabTestCatalog on ot.TestId equals t.Id
            where ot.OrderId == order.Id
            select new { ot.Id, ot.PriceApplied, t.TestName, t.TestCode }
        ).ToListAsync(cancellationToken);

        if (orderTests.Count == 0)
        {
            throw new InvalidOperationException("This lab order has no tests to bill.");
        }

        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var invoice = new BillingInvoice
        {
            PatientId = order.PatientId,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Subtotal = 0,
            DiscountAmount = 0,
            CgstAmount = 0,
            SgstAmount = 0,
            IgstAmount = 0,
            TotalAmount = 0
        };

        _context.BillingInvoices.Add(invoice);

        decimal subtotal = 0;
        decimal totalGst = 0;

        foreach (var test in orderTests)
        {
            var lineTotal = test.PriceApplied;

            var item = new BillingInvoiceItem
            {
                InvoiceId = invoice.Id,
                SourceModule = "Laboratory",
                SourceRecordId = test.Id,
                Description = $"Lab Test: {test.TestName} ({test.TestCode})",
                Quantity = 1,
                UnitPrice = test.PriceApplied,
                GstRatePct = request.GstRatePct,
                LineTotal = lineTotal
            };

            _context.BillingInvoiceItems.Add(item);

            subtotal += lineTotal;
            totalGst += lineTotal * request.GstRatePct / 100m;
        }

        invoice.Subtotal = subtotal;
        invoice.CgstAmount = totalGst / 2m;
        invoice.SgstAmount = totalGst / 2m;
        invoice.IgstAmount = 0;
        invoice.TotalAmount = subtotal - invoice.DiscountAmount + invoice.CgstAmount + invoice.SgstAmount + invoice.IgstAmount;

        await _context.SaveChangesAsync(cancellationToken);

        return invoice.Id;
    }
}