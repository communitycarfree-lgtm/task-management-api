using TaskManagementAPI.Modules.Tasks.Domain.Enums;

namespace TaskManagementAPI.Modules.Tasks.Application.DTOs;

/// <summary>
/// Request DTO for updating a task status.
/// </summary>
public class UpdateTaskStatusRequest
{
    /// <summary>
    /// The new task status.
    /// </summary>
    public TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus Status { get; set; }
}
