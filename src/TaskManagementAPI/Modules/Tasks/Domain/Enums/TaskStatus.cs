namespace TaskManagementAPI.Modules.Tasks.Domain.Enums;

/// <summary>
/// Enumeration of possible task statuses.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// Task has been created but not started.
    /// </summary>
    New = 0,

    /// <summary>
    /// Task is currently being worked on.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Task has been completed.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Task is blocked by another task or dependency.
    /// </summary>
    Blocked = 3,

    /// <summary>
    /// Task has been cancelled and will not be completed.
    /// </summary>
    Cancelled = 4
}
