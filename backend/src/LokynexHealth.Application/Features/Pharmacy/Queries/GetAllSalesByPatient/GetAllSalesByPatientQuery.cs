using MediatR;
using LokynexHealth.Application.Features.Pharmacy.DTOs;

namespace LokynexHealth.Application.Features.Pharmacy.Queries.GetAllSalesByPatient;

public class GetAllSalesByPatientQuery : IRequest<List<SaleDto>>
{
    public Guid PatientId { get; set; }
}