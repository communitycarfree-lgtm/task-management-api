using TaskManagementAPI.Shared.Domain;
using TaskManagementAPI.Modules.Projects.Domain.Enums;

namespace TaskManagementAPI.Modules.Projects.Domain.Entities;

/// <summary>
/// Represents a project entity that contains tasks and team members.
/// </summary>
public class Project : BaseEntity
{
    /// <summary>
    /// The name of the project.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The current status of the project.
    /// </summary>
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    /// <summary>
    /// Collection of project members.
    /// </summary>
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
}
