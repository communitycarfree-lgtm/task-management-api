using System.Collections.Concurrent;

namespace TaskManagementAPI.Shared.Infrastructure.Middleware;

/// <summary>
/// Middleware for implementing rate limiting to prevent abuse and DoS attacks.
/// Uses a sliding window approach with configurable requests per time window.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly int _requestsPerWindow;
    private readonly TimeSpan _timeWindow;
    private static readonly ConcurrentDictionary<string, Queue<DateTime>> _requestHistory = new();

    /// <summary>
    /// Initializes a new instance of the RateLimitingMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="requestsPerWindow">Maximum requests allowed per time window (default: 100).</param>
    /// <param name="timeWindowSeconds">Time window in seconds (default: 60).</param>
    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, int requestsPerWindow = 100, int timeWindowSeconds = 60)
    {
        _next = next;
        _logger = logger;
        _requestsPerWindow = requestsPerWindow;
        _timeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
    }

    /// <summary>
    /// Invokes the middleware to check rate limits for the current request.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        
        if (!IsRequestAllowed(clientId))
        {
            _logger.LogWarning("Rate limit exceeded for client: {ClientId}", clientId);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "Rate limit exceeded. Please try again later." });
            return;
        }

        await _next(context);
    }

    /// <summary>
    /// Gets a unique identifier for the client (IP address or user ID).
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A unique client identifier.</returns>
    private static string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID from claims first
        var userId = context.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userId))
            return $"user:{userId}";

        // Fall back to IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"ip:{ipAddress}";
    }

    /// <summary>
    /// Checks if a request from the given client is allowed based on rate limits.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <returns>True if the request is allowed; otherwise false.</returns>
    private bool IsRequestAllowed(string clientId)
    {
        var now = DateTime.UtcNow;
        var queue = _requestHistory.GetOrAdd(clientId, _ => new Queue<DateTime>());

        lock (queue)
        {
            // Remove old requests outside the time window
            while (queue.Count > 0 && queue.Peek() < now - _timeWindow)
            {
                queue.Dequeue();
            }

            // Check if limit is exceeded
            if (queue.Count >= _requestsPerWindow)
                return false;

            // Add current request
            queue.Enqueue(now);
            return true;
        }
    }
}
