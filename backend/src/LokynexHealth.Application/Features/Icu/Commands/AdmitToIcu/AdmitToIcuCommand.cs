using MediatR;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Application.Features.Icu.Commands.AdmitToIcu;

public class AdmitToIcuCommand : IRequest<Guid>
{
    public Guid AdmissionId { get; set; }
    public IcuUnitType IcuUnitType { get; set; }
    public short? ApacheIiScore { get; set; }
    public short? SofaScore { get; set; }
}