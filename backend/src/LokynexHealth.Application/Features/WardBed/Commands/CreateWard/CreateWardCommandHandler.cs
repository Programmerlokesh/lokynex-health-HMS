using MediatR;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.WardBed.Commands.CreateWard;

public class CreateWardCommandHandler : IRequestHandler<CreateWardCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateWardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateWardCommand request, CancellationToken cancellationToken)
    {
        var ward = new Ward
        {
            WardName = request.WardName,
            WardType = request.WardType,
            Floor = request.Floor,
            IsActive = true
        };

        _context.Wards.Add(ward);
        await _context.SaveChangesAsync(cancellationToken);

        return ward.Id;
    }
}