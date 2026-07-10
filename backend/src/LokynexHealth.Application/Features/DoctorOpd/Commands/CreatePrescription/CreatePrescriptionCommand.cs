using MediatR;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Application.Features.DoctorOpd.Commands.CreatePrescription;

public class CreatePrescriptionCommand : IRequest<Guid>
{
    public Guid VisitId { get; set; }
    public List<PrescriptionItemRequest> Items { get; set; } = new();
}

public class PrescriptionItemRequest
{
    public string DrugName { get; set; } = string.Empty;
    public string? RxnormCode { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public short? DurationDays { get; set; }
    public ScheduleDrugFlag ScheduleFlag { get; set; } = ScheduleDrugFlag.None;
}