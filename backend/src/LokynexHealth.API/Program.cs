using FluentValidation;
using LokynexHealth.Application.Common.Behaviors;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Patients.Commands.CreatePatient;
using LokynexHealth.Infrastructure.Persistence;
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

var app = builder.Build();

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
