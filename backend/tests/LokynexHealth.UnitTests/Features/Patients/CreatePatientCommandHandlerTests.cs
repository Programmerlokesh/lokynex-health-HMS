using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Features.Patients.Commands.CreatePatient;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Infrastructure.Persistence;
using Xunit;

namespace LokynexHealth.UnitTests.Features.Patients;

public class CreatePatientCommandHandlerTests
{
    private static LokynexHealthDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LokynexHealthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LokynexHealthDbContext(options);
    }

    private static async Task<Guid> SeedTenantAsync(LokynexHealthDbContext context)
    {
        var tenant = new Tenant
        {
            HospitalName = "Test Hospital",
            Subdomain = $"test-{Guid.NewGuid().ToString()[..8]}",
            ContactEmail = "test@hospital.com",
            ContactPhone = "9999999999",
            Address = "Test Address",
            Status = TenantStatus.Active,
            SubscriptionStartDate = DateTime.UtcNow
        };

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync(CancellationToken.None);

        return tenant.Id;
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesPatientAndReturnsId()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var tenantId = await SeedTenantAsync(context);
        var handler = new CreatePatientCommandHandler(context);

        var command = new CreatePatientCommand
        {
            TenantId = tenantId,
            FullName = "Test Patient",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Gender.Female,
            PhoneNumber = "9876543210",
            Email = "test@example.com",
            Address = "Test Address"
        };

        // Act
        var patientId = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, patientId);

        var savedPatient = await context.Patients.FindAsync(patientId);
        Assert.NotNull(savedPatient);
        Assert.Equal("Test Patient", savedPatient.FullName);
        Assert.StartsWith("WB-", savedPatient.MedicalRecordNumber);
    }

    [Fact]
    public async Task Handle_ValidCommand_GeneratesUniqueMedicalRecordNumber()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var tenantId = await SeedTenantAsync(context);
        var handler = new CreatePatientCommandHandler(context);

        var command1 = new CreatePatientCommand
        {
            TenantId = tenantId,
            FullName = "Patient One",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Gender.Male,
            PhoneNumber = "9876543210",
            Address = "Address One"
        };

        var command2 = new CreatePatientCommand
        {
            TenantId = tenantId,
            FullName = "Patient Two",
            DateOfBirth = new DateTime(1992, 5, 15),
            Gender = Gender.Female,
            PhoneNumber = "9876543211",
            Address = "Address Two"
        };

        // Act
        var id1 = await handler.Handle(command1, CancellationToken.None);
        var id2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        var patient1 = await context.Patients.FindAsync(id1);
        var patient2 = await context.Patients.FindAsync(id2);

        Assert.NotEqual(patient1!.MedicalRecordNumber, patient2!.MedicalRecordNumber);
    }
}
