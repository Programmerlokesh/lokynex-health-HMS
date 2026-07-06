using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Patients.Commands.CreatePatient;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Infrastructure.Persistence;
using Xunit;

namespace LokynexHealth.UnitTests.Features.Patients;

// Minimal test double — unit tests don't need real multi-tenant resolution
public class TestTenantContext : ITenantContext
{
    public string? SchemaName { get; private set; } = "hms_test";
    public Guid? TenantId { get; private set; }

    public void SetTenant(Guid tenantId, string schemaName)
    {
        TenantId = tenantId;
        SchemaName = schemaName;
    }
}

public class CreatePatientCommandHandlerTests
{
    private static LokynexHealthDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LokynexHealthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LokynexHealthDbContext(options, new TestTenantContext());
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesPatientAndReturnsId()
    {
        var context = CreateInMemoryContext();
        var handler = new CreatePatientCommandHandler(context);

        var command = new CreatePatientCommand
        {
            FullName = "Test Patient",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Gender.Female,
            PhoneNumber = "9876543210",
            Email = "test@example.com",
            Address = "Test Address"
        };

        var patientId = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, patientId);

        var savedPatient = await context.Patients.FindAsync(patientId);
        Assert.NotNull(savedPatient);
        Assert.Equal("Test Patient", savedPatient.FullName);
        Assert.StartsWith("WB-", savedPatient.MedicalRecordNumber);
    }

    [Fact]
    public async Task Handle_ValidCommand_GeneratesUniqueMedicalRecordNumber()
    {
        var context = CreateInMemoryContext();
        var handler = new CreatePatientCommandHandler(context);

        var command1 = new CreatePatientCommand
        {
            FullName = "Patient One",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Gender.Male,
            PhoneNumber = "9876543210",
            Address = "Address One"
        };

        var command2 = new CreatePatientCommand
        {
            FullName = "Patient Two",
            DateOfBirth = new DateTime(1992, 5, 15),
            Gender = Gender.Female,
            PhoneNumber = "9876543211",
            Address = "Address Two"
        };

        var id1 = await handler.Handle(command1, CancellationToken.None);
        var id2 = await handler.Handle(command2, CancellationToken.None);

        var patient1 = await context.Patients.FindAsync(id1);
        var patient2 = await context.Patients.FindAsync(id2);

        Assert.NotEqual(patient1!.MedicalRecordNumber, patient2!.MedicalRecordNumber);
    }
}
