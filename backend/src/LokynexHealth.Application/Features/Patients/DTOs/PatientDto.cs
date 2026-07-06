namespace LokynexHealth.Application.Features.Patients.DTOs;

public class PatientDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string PhoneNumber
    {
        get => Mobile;
        set => Mobile = value;
    }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Uhid { get; set; } = string.Empty;
    public string MedicalRecordNumber
    {
        get => Uhid;
        set => Uhid = value;
    }
}
