namespace TaskManagementAPI.Modules.Projects.Application.DTOs;

/// <summary>
/// Request DTO for creating a new project.
/// </summary>
public class CreateProjectRequest
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
