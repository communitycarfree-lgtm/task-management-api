using TaskManagementAPI.Modules.Projects.Domain.Entities;
using TaskManagementAPI.Modules.Projects.Domain.Enums;
using TaskManagementAPI.Modules.Projects.Infrastructure.Services;
using TaskManagementAPI.Shared.Domain.Interfaces;

namespace TaskManagementAPI.Modules.Projects.Application.Services;

/// <summary>
/// Service for managing projects and project members.
/// </summary>
public class ProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly INotificationService _notificationService;

    /// <summary>
    /// Initializes a new instance of the ProjectService class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="notificationService">The notification service.</param>
    public ProjectService(IProjectRepository projectRepository, INotificationService notificationService)
    {
        _projectRepository = projectRepository;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description.</param>
    /// <returns>The created project.</returns>
    public async Task<Project> CreateProjectAsync(string name, string? description = null)
    {
        var project = new Project
        {
            Name = name,
            Description = description,
            Status = ProjectStatus.Active
        };

        await _projectRepository.AddAsync(project);
        await _notificationService.BroadcastAsync("projects", $"New project created: {project.Name}");

        return project;
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="name">The new project name.</param>
    /// <param name="description">The new project description.</param>
    /// <returns>The updated project.</returns>
    public async Task<Project?> UpdateProjectAsync(Guid projectId, string name, string? description = null)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            return null;

        project.Name = name;
        project.Description = description;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
        await _notificationService.BroadcastAsync($"project-{projectId}", $"Project updated: {project.Name}");

        return project;
    }

    /// <summary>
    /// Soft-deletes a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <returns>True if the project was deleted; otherwise false.</returns>
    public async Task<bool> DeleteProjectAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            return false;

        await _projectRepository.DeleteAsync(projectId);
        await _notificationService.BroadcastAsync($"project-{projectId}", $"Project deleted: {project.Name}");

        return true;
    }

    /// <summary>
    /// Adds a member to a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="role">The member role.</param>
    /// <returns>The created project member.</returns>
    public async Task<ProjectMember> AddMemberAsync(Guid projectId, string userId, ProjectMemberRole role = ProjectMemberRole.Developer)
    {
        var project = await _projectRepository.GetByIdWithMembersAsync(projectId);
        if (project == null)
            throw new InvalidOperationException($"Project with ID {projectId} not found.");

        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };

        project.Members.Add(member);
        await _projectRepository.UpdateAsync(project);
        await _notificationService.BroadcastAsync($"project-{projectId}", $"New member added to project: {userId}");

        return member;
    }

    /// <summary>
    /// Removes a member from a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <returns>True if the member was removed; otherwise false.</returns>
    public async Task<bool> RemoveMemberAsync(Guid projectId, string userId)
    {
        var project = await _projectRepository.GetByIdWithMembersAsync(projectId);
        if (project == null)
            return false;

        var member = project.Members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
            return false;

        project.Members.Remove(member);
        await _projectRepository.UpdateAsync(project);
        await _notificationService.BroadcastAsync($"project-{projectId}", $"Member removed from project: {userId}");

        return true;
    }

    /// <summary>
    /// Gets a project by ID with all members.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <returns>The project with members, or null if not found.</returns>
    public async Task<Project?> GetProjectWithMembersAsync(Guid projectId)
    {
        return await _projectRepository.GetByIdWithMembersAsync(projectId);
    }

    /// <summary>
    /// Gets all projects for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>A collection of projects the user is a member of.</returns>
    public async Task<IEnumerable<Project>> GetUserProjectsAsync(string userId)
    {
        return await _projectRepository.GetUserProjectsAsync(userId);
    }

    /// <summary>
    /// Gets projects with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A tuple containing the projects and total count.</returns>
    public async Task<(IEnumerable<Project> Projects, int TotalCount)> GetProjectsPagedAsync(int pageNumber, int pageSize)
    {
        return await _projectRepository.GetProjectsPagedAsync(pageNumber, pageSize);
    }
}
