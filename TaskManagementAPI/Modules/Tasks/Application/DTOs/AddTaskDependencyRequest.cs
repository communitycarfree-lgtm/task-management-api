namespace TaskManagementAPI.Modules.Tasks.Application.DTOs;

/// <summary>
/// Request DTO for adding a task dependency.
/// </summary>
public class AddTaskDependencyRequest
{
    /// <summary>
    /// The ID of the task that is blocking.
    /// </summary>
    public Guid BlockedByTaskId { get; set; }
}
