using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;

namespace LokynexHealth.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<Tenant> Tenants { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}