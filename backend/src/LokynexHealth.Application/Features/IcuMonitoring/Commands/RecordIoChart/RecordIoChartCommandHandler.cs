using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.RecordIoChart;

public class RecordIoChartCommandHandler : IRequestHandler<RecordIoChartCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RecordIoChartCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RecordIoChartCommand request, CancellationToken cancellationToken)
    {
        var admissionExists = await _context.IcuAdmissions
            .AnyAsync(a => a.Id == request.IcuAdmissionId, cancellationToken);

        if (!admissionExists)
        {
            throw new KeyNotFoundException($"ICU admission with Id '{request.IcuAdmissionId}' was not found.");
        }

        var chart = new IcuIoChart
        {
            IcuAdmissionId = request.IcuAdmissionId,
            ChartDate = request.ChartDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            IntakeMl = request.IntakeMl,
            OutputMl = request.OutputMl,
            RecordedBy = request.RecordedBy
        };

        _context.IcuIoCharts.Add(chart);
        await _context.SaveChangesAsync(cancellationToken);

        return chart.Id;
    }
}