using MediatR;

namespace LokynexHealth.Application.Features.DoctorOpd.Commands.CreateOpdVisit;

public class CreateOpdVisitCommand : IRequest<Guid>
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? TokenId { get; set; }
    public string? SchemeTag { get; set; }
    public string? ChiefComplaint { get; set; }
}
