using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
        var requestId = context.Request.Headers.TryGetValue("X-Request-ID", out var requestIdHeader)
            ? requestIdHeader.ToString()
            : traceId;

        _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}, RequestId: {RequestId}",
            traceId, requestId);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            ErrorCode = "INTERNAL_SERVER_ERROR",
            Message = "An unexpected error occurred. Please try again later.",
            Timestamp = DateTime.UtcNow,
            TraceId = traceId,
            RequestId = requestId
        };

        response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(errorResponse, options);

        return response.WriteAsync(json);
    }
}

/// <summary>
/// Standard error response format for all API errors.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Machine-readable error code for programmatic handling.
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Validation errors if applicable.
    /// </summary>
    public List<ValidationError>? Errors { get; set; }

    /// <summary>
    /// UTC timestamp when the error occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Trace ID for error correlation.
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// Request ID for error correlation.
    /// </summary>
    public string RequestId { get; set; } = string.Empty;
}

/// <summary>
/// Represents a single validation error.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// The field name that failed validation.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// The validation error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
