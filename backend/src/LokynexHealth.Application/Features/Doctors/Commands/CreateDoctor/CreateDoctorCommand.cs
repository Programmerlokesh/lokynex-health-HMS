using MediatR;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Application.Features.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommand : IRequest<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DoctorSpecialization Specialization { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
}
