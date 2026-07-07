using MediatR;

namespace LokynexHealth.Application.Features.WardBed.Commands.AdmitPatient;

public class AdmitPatientCommand : IRequest<Guid>
{
    public Guid PatientId { get; set; }
    public Guid BedId { get; set; }
    public Guid? AdmittingDoctorId { get; set; }
    public string? PmjayPackageCode { get; set; }
}