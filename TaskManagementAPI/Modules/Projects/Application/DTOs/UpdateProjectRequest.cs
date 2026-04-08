namespace TaskManagementAPI.Modules.Projects.Application.DTOs;

/// <summary>
/// Request DTO for updating a project.
/// </summary>
public class UpdateProjectRequest
{
    /// <summary>
    /// The name of the project.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the project.
    /// </summary>
    public string? Description { get; set; }
}
