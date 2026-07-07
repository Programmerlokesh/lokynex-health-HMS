using MediatR;

namespace LokynexHealth.Application.Features.WardBed.Commands.DischargePatient;

public class DischargePatientCommand : IRequest<Guid>
{
    public Guid AdmissionId { get; set; }
}