using MediatR;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommand : IRequest<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
}
