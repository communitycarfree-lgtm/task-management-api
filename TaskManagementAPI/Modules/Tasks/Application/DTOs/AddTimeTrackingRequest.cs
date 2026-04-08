namespace TaskManagementAPI.Modules.Tasks.Application.DTOs;

/// <summary>
/// Request DTO for adding a time tracking entry.
/// </summary>
public class AddTimeTrackingRequest
{
    /// <summary>
    /// The number of hours.
    /// </summary>
    public decimal Hours { get; set; }

    /// <summary>
    /// The date of the entry.
    /// </summary>
    public DateTime Date { get; set; }
}
