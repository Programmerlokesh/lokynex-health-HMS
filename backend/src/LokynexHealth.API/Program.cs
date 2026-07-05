using FluentValidation;
using LokynexHealth.API.Middleware;
using LokynexHealth.Application.Common.Behaviors;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Patients.Commands.CreatePatient;
using LokynexHealth.Infrastructure.Persistence;
using LokynexHealth.Infrastructure.Persistence.Platform;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreatePatientCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(CreatePatientCommand).Assembly);

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<LokynexHealthDbContext>());

builder.Services.AddDbContext<LokynexHealthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPlatformDbContext>(provider =>
    provider.GetRequiredService<PlatformDbContext>());

builder.Services.AddDbContext<PlatformDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PlatformConnection")));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
