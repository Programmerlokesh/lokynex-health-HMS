using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class Patient : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
    public string MedicalRecordNumber { get; set; } = string.Empty;
}
