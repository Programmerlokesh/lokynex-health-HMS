using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class Doctor : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DoctorSpecialization Specialization { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}
