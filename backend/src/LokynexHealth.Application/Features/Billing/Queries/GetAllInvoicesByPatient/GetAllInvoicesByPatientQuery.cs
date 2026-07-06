using MediatR;
using LokynexHealth.Application.Features.Billing.DTOs;

namespace LokynexHealth.Application.Features.Billing.Queries.GetAllInvoicesByPatient;

public class GetAllInvoicesByPatientQuery : IRequest<List<InvoiceDto>>
{
    public Guid PatientId { get; set; }
}
