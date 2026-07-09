using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class IcuAdmission : BaseEntity
{
    public Guid AdmissionId { get; set; }
    public IcuUnitType IcuUnitType { get; set; }
    public DateTimeOffset AdmittedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DischargedAt { get; set; }
    public short? ApacheIiScore { get; set; }
    public short? SofaScore { get; set; }
    public RecordStatus Status { get; set; } = RecordStatus.Active;
}