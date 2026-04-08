using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Projects.Domain.Entities;
using TaskManagementAPI.Modules.Projects.Infrastructure.Persistence.Configurations;
using TaskManagementAPI.Shared.Infrastructure;

namespace TaskManagementAPI.Modules.Projects.Infrastructure.Persistence;

/// <summary>
/// DbContext for the Projects module.
/// Manages Project and ProjectMember entities.
/// </summary>
public class ProjectsDbContext : BaseDbContext
{
    /// <summary>
    /// Initializes a new instance of the ProjectsDbContext class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    public ProjectsDbContext(DbContextOptions<ProjectsDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet for projects.
    /// </summary>
    public DbSet<Project> Projects { get; set; } = null!;

    /// <summary>
    /// DbSet for project members.
    /// </summary>
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;

    /// <summary>
    /// Configures the model for the Projects module.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectMemberConfiguration());
    }
}
