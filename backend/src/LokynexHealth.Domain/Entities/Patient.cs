using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class Patient : BaseEntity
{
    public string Uhid { get; set; } = string.Empty;
    public string? AbhaId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    public byte[]? AadhaarNumberEnc { get; set; }
    public string? AadhaarLast4 { get; set; }
    public string? PanNumber { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string? AltMobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string PreferredLanguage { get; set; } = "en";
    public string? BloodGroup { get; set; }
    public Guid? IsDuplicateOf { get; set; }
    public Guid? RegisteredAtTenantBranch { get; set; }
    public RecordStatus Status { get; set; } = RecordStatus.Active;

    public string MedicalRecordNumber
    {
        get => Uhid;
        set => Uhid = value;
    }

    public string PhoneNumber
    {
        get => Mobile;
        set => Mobile = value;
    }
}
