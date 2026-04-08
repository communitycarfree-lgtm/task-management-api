using TaskManagementAPI.Shared.Domain;
using TaskManagementAPI.Modules.Projects.Domain.Enums;

namespace TaskManagementAPI.Modules.Projects.Domain.Entities;

/// <summary>
/// Represents a member of a project with a specific role.
/// </summary>
public class ProjectMember : BaseEntity
{
    /// <summary>
    /// The ID of the project.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// The ID of the user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The role of the member within the project.
    /// </summary>
    public ProjectMemberRole Role { get; set; } = ProjectMemberRole.Developer;

    /// <summary>
    /// The date and time when the member joined the project.
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property to the project.
    /// </summary>
    public Project? Project { get; set; }
}
