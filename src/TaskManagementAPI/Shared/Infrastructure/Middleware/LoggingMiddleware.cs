using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TaskManagementAPI.Shared.Infrastructure.Middleware;

/// <summary>
/// Middleware for structured logging of HTTP requests and responses.
/// Logs request details, response status, and execution time with context information.
/// </summary>
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the LoggingMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to log HTTP requests and responses.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers.TryGetValue("X-Request-ID", out var requestIdHeader)
            ? requestIdHeader.ToString()
            : context.TraceIdentifier;

        var userId = context.User?.FindFirst("sub")?.Value ?? "anonymous";

        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("Timestamp", DateTime.UtcNow))
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "HTTP Request: {Method} {Path} from {RemoteIP}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress);

            var originalBodyStream = context.Response.Body;

            try
            {
                await _next(context);

                stopwatch.Stop();

                _logger.LogInformation(
                    "HTTP Response: {Method} {Path} - Status {StatusCode} - Duration {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex,
                    "HTTP Request failed: {Method} {Path} - Duration {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }
}

/// <summary>
/// Helper class for managing log context properties.
/// </summary>
public static class LogContext
{
    /// <summary>
    /// Pushes a property into the log context.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    /// <returns>A disposable that removes the property when disposed.</returns>
    public static IDisposable PushProperty(string name, object? value)
    {
        return new LogContextProperty(name, value);
    }

    private class LogContextProperty : IDisposable
    {
        private readonly string _name;

        public LogContextProperty(string name, object? value)
        {
            _name = name;
            // In a real implementation, this would integrate with Serilog's LogContext
            // For now, this is a placeholder for the middleware structure
        }

        public void Dispose()
        {
            // Cleanup
        }
    }
}
