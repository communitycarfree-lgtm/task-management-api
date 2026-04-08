using TaskManagementAPI.Modules.Projects.Domain.Entities;
using TaskManagementAPI.Modules.Projects.Domain.Enums;
using TaskManagementAPI.Modules.Projects.Infrastructure.Services;
using TaskManagementAPI.Shared.Domain.Interfaces;
using TaskManagementAPI.Shared.Domain.Services;

namespace TaskManagementAPI.Modules.Projects.Application.Services;

/// <summary>
/// Service for managing projects and project members.
/// Handles project creation, updates, member management, and SEO-friendly slug generation.
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
    /// Creates a new project with an auto-generated SEO-friendly slug.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description.</param>
    /// <returns>The created project with generated slug.</returns>
    public async System.Threading.Tasks.Task<Project> CreateProjectAsync(string name, string? description = null)
    {
        // Generate slug for SEO (optional - may fail if database doesn't support it yet)
        string? slug = null;
        try
        {
            var baseSlug = SlugService.GenerateSlug(name);
            var existingSlugs = await _projectRepository.GetAllSlugsAsync();
            slug = SlugService.GenerateUniqueSlug(baseSlug, existingSlugs ?? Enumerable.Empty<string>());
        }
        catch
        {
            // Slug generation failed - continue without it
        }

        var project = new Project
        {
            Name = name,
            Slug = slug,
            Description = description,
            Status = ProjectStatus.Active
        };

        await _projectRepository.AddAsync(project);
        await _notificationService.BroadcastAsync("projects", $"New project created: {project.Name}");

        return project;
    }

    /// <summary>
    /// Updates an existing project and regenerates slug if name changes.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="name">The new project name.</param>
    /// <param name="description">The new project description.</param>
    /// <returns>The updated project, or null if not found.</returns>
    public async System.Threading.Tasks.Task<Project?> UpdateProjectAsync(Guid projectId, string name, string? description = null)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            return null;

        // Regenerate slug if name changed
        if (project.Name != name)
        {
            var baseSlug = SlugService.GenerateSlug(name);
            var existingSlugs = (await _projectRepository.GetAllSlugsAsync())
                .Where(s => s != project.Slug) // Exclude current slug
                .ToList();
            project.Slug = SlugService.GenerateUniqueSlug(baseSlug, existingSlugs);
        }

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
    public async System.Threading.Tasks.Task<bool> DeleteProjectAsync(Guid projectId)
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
    public async System.Threading.Tasks.Task<ProjectMember> AddMemberAsync(Guid projectId, string userId, ProjectMemberRole role = ProjectMemberRole.Developer)
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
    public async System.Threading.Tasks.Task<bool> RemoveMemberAsync(Guid projectId, string userId)
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
    public async System.Threading.Tasks.Task<Project?> GetProjectWithMembersAsync(Guid projectId)
    {
        return await _projectRepository.GetByIdWithMembersAsync(projectId);
    }

    /// <summary>
    /// Gets a project by its SEO-friendly slug.
    /// </summary>
    /// <param name="slug">The project slug.</param>
    /// <returns>The project, or null if not found.</returns>
    public async System.Threading.Tasks.Task<Project?> GetProjectBySlugAsync(string slug)
    {
        return await _projectRepository.GetBySlugAsync(slug);
    }

    /// <summary>
    /// Gets all projects for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>A collection of projects the user is a member of.</returns>
    public async System.Threading.Tasks.Task<IEnumerable<Project>> GetUserProjectsAsync(string userId)
    {
        return await _projectRepository.GetUserProjectsAsync(userId);
    }

    /// <summary>
    /// Gets projects with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A tuple containing the projects and total count.</returns>
    public async System.Threading.Tasks.Task<(IEnumerable<Project> Projects, int TotalCount)> GetProjectsPagedAsync(int pageNumber, int pageSize)
    {
        return await _projectRepository.GetProjectsPagedAsync(pageNumber, pageSize);
    }
}
