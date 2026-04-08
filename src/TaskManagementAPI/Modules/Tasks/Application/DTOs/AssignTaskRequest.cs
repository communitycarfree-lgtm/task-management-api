namespace TaskManagementAPI.Modules.Tasks.Application.DTOs;

/// <summary>
/// Request DTO for assigning a task to a user.
/// </summary>
public class AssignTaskRequest
{
    /// <summary>
    /// The assignee ID.
    /// </summary>
    public string AssigneeId { get; set; } = string.Empty;
}
