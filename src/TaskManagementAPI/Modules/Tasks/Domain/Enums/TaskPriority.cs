namespace TaskManagementAPI.Modules.Tasks.Domain.Enums;

/// <summary>
/// Enumeration of possible task priorities.
/// </summary>
public enum TaskPriority
{
    /// <summary>
    /// Low priority task.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium priority task.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High priority task.
    /// </summary>
    High = 2,

    /// <summary>
    /// Critical priority task requiring immediate attention.
    /// </summary>
    Critical = 3
}
