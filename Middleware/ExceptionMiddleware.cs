using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EP.API.Middleware;

/// <summary>
/// Global exception handling middleware.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await WriteErrorResponse(context, "Not Found", ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Bad request");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await WriteErrorResponse(context, "Bad Request", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict");
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await WriteErrorResponse(context, "Conflict", ex.Message);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed");
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await WriteErrorResponse(context, "Conflict", "The requested change conflicts with existing data.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await WriteErrorResponse(context, "Internal Server Error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, string title, string detail)
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            type = "about:blank",
            title,
            status = context.Response.StatusCode,
            detail,
            traceId = context.TraceIdentifier
        };
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
