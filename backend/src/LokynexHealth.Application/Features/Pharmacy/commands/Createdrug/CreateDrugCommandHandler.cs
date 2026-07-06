using MediatR;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Pharmacy.Commands.CreateDrug;

public class CreateDrugCommandHandler : IRequestHandler<CreateDrugCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateDrugCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDrugCommand request, CancellationToken cancellationToken)
    {
        var drug = new PharmacyDrugCatalog
        {
            DrugName = request.DrugName,
            GenericName = request.GenericName,
            IsJanAushadhiGeneric = request.IsJanAushadhiGeneric,
            HsnCode = request.HsnCode,
            GstRatePct = request.GstRatePct,
            ScheduleFlag = request.ScheduleFlag,
            UnitOfMeasure = request.UnitOfMeasure,
            IsActive = true
        };

        _context.PharmacyDrugCatalog.Add(drug);
        await _context.SaveChangesAsync(cancellationToken);

        return drug.Id;
    }
}