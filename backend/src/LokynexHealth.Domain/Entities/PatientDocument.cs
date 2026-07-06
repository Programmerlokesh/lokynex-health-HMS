using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class PatientDocument : BaseEntity
{
    public Guid PatientId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? OcrExtractedJson { get; set; }
    public decimal? OcrConfidence { get; set; }
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
}
