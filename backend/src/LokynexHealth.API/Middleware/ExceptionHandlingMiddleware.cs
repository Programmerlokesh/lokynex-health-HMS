using System.Net;
using System.Text.Json;
using FluentValidation;

namespace LokynexHealth.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Errors}", ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errors = ex.Errors.Select(e => e.ErrorMessage);
            var result = JsonSerializer.Serialize(new { errors });
            await context.Response.WriteAsync(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Resource not found: {Message}", ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            var result = JsonSerializer.Serialize(new { error = ex.Message });
            await context.Response.WriteAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new { error = "An unexpected error occurred. Please try again later." });
            await context.Response.WriteAsync(result);
        }
    }
}
