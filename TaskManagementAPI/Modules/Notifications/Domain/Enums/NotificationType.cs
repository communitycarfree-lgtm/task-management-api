namespace TaskManagementAPI.Modules.Notifications.Domain.Enums;

/// <summary>
/// Enumeration of notification types.
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// User has been assigned to a task.
    /// </summary>
    TaskAssigned = 0,

    /// <summary>
    /// A task has been completed.
    /// </summary>
    TaskCompleted = 1,

    /// <summary>
    /// User has been mentioned in a task.
    /// </summary>
    TaskMentioned = 2,

    /// <summary>
    /// Task due date is approaching.
    /// </summary>
    DueDateApproaching = 3,

    /// <summary>
    /// Task has critical priority.
    /// </summary>
    CriticalPriority = 4
}
