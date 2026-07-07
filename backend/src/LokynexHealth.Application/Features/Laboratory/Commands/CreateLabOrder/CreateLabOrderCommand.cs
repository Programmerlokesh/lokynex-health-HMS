using MediatR;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Application.Features.Laboratory.Commands.CreateLabOrder;

public class CreateLabOrderCommand : IRequest<Guid>
{
    public Guid PatientId { get; set; }
    public Guid? OrderingDoctorId { get; set; }
    public Guid? SourceVisitId { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Routine;
    public string? SchemeTag { get; set; }
    public List<Guid> TestIds { get; set; } = new();
}