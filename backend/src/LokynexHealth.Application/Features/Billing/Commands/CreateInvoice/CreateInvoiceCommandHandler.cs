using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Billing.Commands.CreateInvoice;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateInvoiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

        if (!patientExists)
        {
            throw new KeyNotFoundException($"Patient with Id '{request.PatientId}' was not found.");
        }

        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var invoice = new BillingInvoice
        {
            PatientId = request.PatientId,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateTime.UtcNow.Date,
            Subtotal = 0,
            DiscountAmount = 0,
            CgstAmount = 0,
            SgstAmount = 0,
            IgstAmount = 0,
            TotalAmount = 0
        };

        _context.BillingInvoices.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        return invoice.Id;
    }
}
