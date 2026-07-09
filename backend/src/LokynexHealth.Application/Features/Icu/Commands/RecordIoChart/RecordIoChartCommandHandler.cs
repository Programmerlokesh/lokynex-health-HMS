using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Icu.Commands.RecordIoChart;

public class RecordIoChartCommandHandler : IRequestHandler<RecordIoChartCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RecordIoChartCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RecordIoChartCommand request, CancellationToken cancellationToken)
    {
        var icuAdmissionExists = await _context.IcuAdmissions
            .AnyAsync(i => i.Id == request.IcuAdmissionId, cancellationToken);

        if (!icuAdmissionExists)
        {
            throw new KeyNotFoundException($"ICU admission with Id '{request.IcuAdmissionId}' was not found.");
        }

        var chartDate = request.ChartDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var existingChart = await _context.IcuIoCharts
            .FirstOrDefaultAsync(c => c.IcuAdmissionId == request.IcuAdmissionId && c.ChartDate == chartDate, cancellationToken);

        if (existingChart is not null)
        {
            existingChart.IntakeMl += request.IntakeMl;
            existingChart.OutputMl += request.OutputMl;
            existingChart.RecordedBy = request.RecordedBy ?? existingChart.RecordedBy;

            await _context.SaveChangesAsync(cancellationToken);
            return existingChart.Id;
        }

        var chart = new IcuIoChart
        {
            IcuAdmissionId = request.IcuAdmissionId,
            ChartDate = chartDate,
            IntakeMl = request.IntakeMl,
            OutputMl = request.OutputMl,
            RecordedBy = request.RecordedBy
        };

        _context.IcuIoCharts.Add(chart);
        await _context.SaveChangesAsync(cancellationToken);

        return chart.Id;
    }
}