using TaskManagementAPI.Modules.Tasks.Domain.Enums;

namespace TaskManagementAPI.Modules.Tasks.Application.DTOs;

/// <summary>
/// Request DTO for updating a task.
/// </summary>
public class UpdateTaskRequest
{
    /// <summary>
    /// The task title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The task description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The task priority.
    /// </summary>
    public TaskPriority? Priority { get; set; }

    /// <summary>
    /// The task due date.
    /// </summary>
    public DateTime? DueDate { get; set; }
}
