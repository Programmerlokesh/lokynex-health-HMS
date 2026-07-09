using MediatR;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.CreateIcuAdmission;

public class CreateIcuAdmissionCommand : IRequest<Guid>
{
    public Guid AdmissionId { get; set; }
    public IcuUnitType IcuUnitType { get; set; }
    public short? ApacheIiScore { get; set; }
    public short? SofaScore { get; set; }
}