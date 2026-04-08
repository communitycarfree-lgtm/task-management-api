using TaskManagementAPI.Shared.Domain;

namespace TaskManagementAPI.Modules.Tasks.Domain.Entities;

/// <summary>
/// Represents a dependency relationship between two tasks.
/// A task can be blocked by another task.
/// </summary>
public class TaskDependency : BaseEntity
{
    /// <summary>
    /// The ID of the task that is blocked.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// The task that is blocked.
    /// </summary>
    public WorkTask? Task { get; set; }

    /// <summary>
    /// The ID of the task that is blocking.
    /// </summary>
    public Guid BlockedByTaskId { get; set; }

    /// <summary>
    /// The task that is blocking.
    /// </summary>
    public WorkTask? BlockedByTask { get; set; }
}
