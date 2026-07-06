using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class OpdTokenQueue : BaseEntity
{
    public string TokenNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateOnly QueueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string Status { get; set; } = "waiting";
    public DateTimeOffset CheckedInAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CalledAt { get; set; }
}
