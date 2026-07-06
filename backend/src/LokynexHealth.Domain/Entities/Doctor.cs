using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class Doctor : BaseEntity
{
    public Guid? EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string NmcRegistrationNo { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? AadhaarPkiCertRef { get; set; }
    public bool IsActive { get; set; } = true;

    public string RegistrationNumber
    {
        get => NmcRegistrationNo;
        set => NmcRegistrationNo = value;
    }

    public bool IsAvailable
    {
        get => IsActive;
        set => IsActive = value;
    }
}
