using TaskManagementAPI.Modules.Projects.Domain.Enums;

namespace TaskManagementAPI.Modules.Projects.Application.DTOs;

/// <summary>
/// Response DTO for a project member.
/// </summary>
public class ProjectMemberDto
{
    /// <summary>
    /// The member ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The user ID.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The member's role in the project.
    /// </summary>
    public ProjectMemberRole Role { get; set; }

    /// <summary>
    /// The date and time when the member joined the project.
    /// </summary>
    public DateTime JoinedAt { get; set; }
}
