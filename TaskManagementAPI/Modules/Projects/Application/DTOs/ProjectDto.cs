using TaskManagementAPI.Modules.Projects.Domain.Enums;

namespace TaskManagementAPI.Modules.Projects.Application.DTOs;

/// <summary>
/// Response DTO for a project.
/// </summary>
public class ProjectDto
{
    /// <summary>
    /// The project ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The project name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The project description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The project status.
    /// </summary>
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// The date and time when the project was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time when the project was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// The project members.
    /// </summary>
    public ICollection<ProjectMemberDto> Members { get; set; } = new List<ProjectMemberDto>();
}
