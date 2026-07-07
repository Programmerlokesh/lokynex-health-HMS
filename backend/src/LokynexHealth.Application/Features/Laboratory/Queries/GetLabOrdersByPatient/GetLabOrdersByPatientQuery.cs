using MediatR;
using LokynexHealth.Application.Features.Laboratory.DTOs;

namespace LokynexHealth.Application.Features.Laboratory.Queries.GetLabOrdersByPatient;

public class GetLabOrdersByPatientQuery : IRequest<List<LabOrderSummaryDto>>
{
    public Guid PatientId { get; set; }
}