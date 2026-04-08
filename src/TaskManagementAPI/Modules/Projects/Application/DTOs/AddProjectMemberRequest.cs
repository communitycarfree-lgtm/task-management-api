using TaskManagementAPI.Modules.Projects.Domain.Enums;

namespace TaskManagementAPI.Modules.Projects.Application.DTOs;

/// <summary>
/// Request DTO for adding a member to a project.
/// </summary>
public class AddProjectMemberRequest
{
    /// <summary>
    /// The user ID to add to the project.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The role for the new member.
    /// </summary>
    public ProjectMemberRole Role { get; set; } = ProjectMemberRole.Developer;
}
