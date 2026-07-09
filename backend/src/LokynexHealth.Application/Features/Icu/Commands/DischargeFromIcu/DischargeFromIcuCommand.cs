using MediatR;

namespace LokynexHealth.Application.Features.Icu.Commands.DischargeFromIcu;

public class DischargeFromIcuCommand : IRequest<Guid>
{
    public Guid IcuAdmissionId { get; set; }
}