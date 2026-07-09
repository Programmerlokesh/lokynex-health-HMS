using MediatR;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.DischargeIcuAdmission;

public class DischargeIcuAdmissionCommand : IRequest<Guid>
{
    public Guid IcuAdmissionId { get; set; }
}