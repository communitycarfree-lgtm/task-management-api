using TaskManagementAPI.Modules.Tasks.Domain.Enums;

namespace TaskManagementAPI.Modules.Tasks.Application.DTOs;

/// <summary>
/// Response DTO for a task.
/// </summary>
public class TaskDto
{
    /// <summary>
    /// The task ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The project ID.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// The task title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The SEO-friendly URL slug for the task.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// The task description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The task status.
    /// </summary>
    public TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus Status { get; set; }

    /// <summary>
    /// The task priority.
    /// </summary>
    public TaskPriority Priority { get; set; }

    /// <summary>
    /// The assignee ID.
    /// </summary>
    public string? AssigneeId { get; set; }

    /// <summary>
    /// The task due date.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// The creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
