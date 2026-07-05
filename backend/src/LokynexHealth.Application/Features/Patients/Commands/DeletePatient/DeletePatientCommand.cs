using MediatR;

namespace LokynexHealth.Application.Features.Patients.Commands.DeletePatient;

public class DeletePatientCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
