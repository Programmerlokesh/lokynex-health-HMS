using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;

namespace LokynexHealth.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<Doctor> Doctors { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
