using MediatR;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Laboratory.Commands.CreateLabTest;

public class CreateLabTestCommandHandler : IRequestHandler<CreateLabTestCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateLabTestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateLabTestCommand request, CancellationToken cancellationToken)
    {
        var test = new LabTestCatalog
        {
            TestCode = request.TestCode,
            TestName = request.TestName,
            LoincCode = request.LoincCode,
            SpecimenType = request.SpecimenType,
            NablPanel = request.NablPanel,
            StandardPrice = request.StandardPrice,
            CghsPrice = request.CghsPrice,
            TatHoursStd = request.TatHoursStd,
            IsActive = true
        };

        _context.LabTestCatalog.Add(test);
        await _context.SaveChangesAsync(cancellationToken);

        return test.Id;
    }
}