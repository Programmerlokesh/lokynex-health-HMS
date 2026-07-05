namespace LokynexHealth.Application.Features.Doctors.DTOs;

public class DoctorDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}
