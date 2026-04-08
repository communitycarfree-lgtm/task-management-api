using TaskManagementAPI.Modules.Tasks.Domain.Enums;
using TaskManagementAPI.Shared.Domain;

namespace TaskManagementAPI.Modules.Tasks.Domain.Entities;

/// <summary>
/// Represents a task within a project.
/// </summary>
public class WorkTask : BaseEntity
{
    /// <summary>
    /// The ID of the project this task belongs to.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// The title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The current status of the task.
    /// </summary>
    public TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus Status { get; set; } = TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus.New;

    /// <summary>
    /// The priority level of the task.
    /// </summary>
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    /// <summary>
    /// The ID of the user assigned to this task.
    /// Null if the task is not assigned.
    /// </summary>
    public string? AssigneeId { get; set; }

    /// <summary>
    /// The due date for the task.
    /// Null if no due date is set.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Collection of tasks that block this task from being completed.
    /// </summary>
    public ICollection<TaskDependency> BlockedByDependencies { get; set; } = new List<TaskDependency>();

    /// <summary>
    /// Collection of tasks that are blocked by this task.
    /// </summary>
    public ICollection<TaskDependency> BlockingDependencies { get; set; } = new List<TaskDependency>();

    /// <summary>
    /// Collection of time tracking entries for this task.
    /// </summary>
    public ICollection<TimeTrackingEntry> TimeTrackingEntries { get; set; } = new List<TimeTrackingEntry>();
}
