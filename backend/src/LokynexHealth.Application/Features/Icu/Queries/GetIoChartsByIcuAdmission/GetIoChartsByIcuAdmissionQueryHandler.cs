using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Icu.DTOs;

namespace LokynexHealth.Application.Features.Icu.Queries.GetIoChartsByIcuAdmission;

public class GetIoChartsByIcuAdmissionQueryHandler : IRequestHandler<GetIoChartsByIcuAdmissionQuery, List<IcuIoChartDto>>
{
    private readonly IApplicationDbContext _context;

    public GetIoChartsByIcuAdmissionQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IcuIoChartDto>> Handle(GetIoChartsByIcuAdmissionQuery request, CancellationToken cancellationToken)
    {
        return await _context.IcuIoCharts
            .AsNoTracking()
            .Where(c => c.IcuAdmissionId == request.IcuAdmissionId)
            .OrderByDescending(c => c.ChartDate)
            .Select(c => new IcuIoChartDto
            {
                Id = c.Id,
                IcuAdmissionId = c.IcuAdmissionId,
                ChartDate = c.ChartDate,
                IntakeMl = c.IntakeMl,
                OutputMl = c.OutputMl,
                NetBalanceMl = c.IntakeMl - c.OutputMl,
                RecordedBy = c.RecordedBy
            })
            .ToListAsync(cancellationToken);
    }
}