using TaskManagementAPI.Shared.Domain;

namespace TaskManagementAPI.Modules.Tasks.Domain.Entities;

/// <summary>
/// Represents a time tracking entry for a task.
/// Records the number of hours spent on a task by a user on a specific date.
/// </summary>
public class TimeTrackingEntry : BaseEntity
{
    /// <summary>
    /// The ID of the task being tracked.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// The task being tracked.
    /// </summary>
    public WorkTask? Task { get; set; }

    /// <summary>
    /// The ID of the user who tracked the time.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The number of hours spent on the task.
    /// </summary>
    public decimal Hours { get; set; }

    /// <summary>
    /// The date the time was tracked.
    /// </summary>
    public DateTime Date { get; set; }
}
