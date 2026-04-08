namespace TaskManagementAPI.Shared.Application.DTOs;

/// <summary>
/// Standard error response DTO for consistent error handling across the API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the error occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets additional error details (optional).
    /// </summary>
    public Dictionary<string, object>? Details { get; set; }

    /// <summary>
    /// Gets or sets the request path that caused the error.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the trace ID for error tracking.
    /// </summary>
    public string? TraceId { get; set; }
}
