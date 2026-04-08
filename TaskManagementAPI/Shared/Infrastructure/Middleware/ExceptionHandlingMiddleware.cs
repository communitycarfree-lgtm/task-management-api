using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TaskManagementAPI.Shared.Application.DTOs;

namespace TaskManagementAPI.Shared.Infrastructure.Middleware;

/// <summary>
/// Global exception handling middleware that catches all unhandled exceptions,
/// formats them consistently, and returns appropriate HTTP responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the ExceptionHandlingMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles an exception by logging it and returning a formatted error response.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The exception to handle.</param>
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", traceId);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Code = "INTERNAL_SERVER_ERROR",
            Message = "An unexpected error occurred. Please try again later.",
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Timestamp = DateTime.UtcNow,
            TraceId = traceId,
            Path = context.Request.Path
        };

        response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(errorResponse, options);

        return response.WriteAsync(json);
    }
}
