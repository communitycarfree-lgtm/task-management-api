using TaskManagementAPI.Modules.Projects.Domain.Entities;
using TaskManagementAPI.Shared.Domain.Interfaces;

namespace TaskManagementAPI.Modules.Projects.Infrastructure.Services;

/// <summary>
/// Repository interface for Project entity with custom query methods.
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
    /// <summary>
    /// Gets a project by ID with all associated members.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <returns>The project with members, or null if not found.</returns>
    Task<Project?> GetByIdWithMembersAsync(Guid id);

    /// <summary>
    /// Gets all projects for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>A collection of projects the user is a member of.</returns>
    Task<IEnumerable<Project>> GetUserProjectsAsync(string userId);

    /// <summary>
    /// Gets projects with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A tuple containing the projects and total count.</returns>
    Task<(IEnumerable<Project> Projects, int TotalCount)> GetProjectsPagedAsync(int pageNumber, int pageSize);
}
