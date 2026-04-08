using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Tasks.Domain.Entities;
using TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Configurations;
using TaskManagementAPI.Shared.Infrastructure;

namespace TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence;

/// <summary>
/// DbContext for the Tasks module.
/// Manages Task, TaskDependency, and TimeTrackingEntry entities.
/// </summary>
public class TasksDbContext : BaseDbContext
{
    /// <summary>
    /// Initializes a new instance of the TasksDbContext class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet for tasks.
    /// </summary>
    public DbSet<WorkTask> Tasks { get; set; } = null!;

    /// <summary>
    /// DbSet for task dependencies.
    /// </summary>
    public DbSet<TaskDependency> TaskDependencies { get; set; } = null!;

    /// <summary>
    /// DbSet for time tracking entries.
    /// </summary>
    public DbSet<TimeTrackingEntry> TimeTrackingEntries { get; set; } = null!;

    /// <summary>
    /// Configures the model for the Tasks module.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
        modelBuilder.ApplyConfiguration(new TaskDependencyConfiguration());
        modelBuilder.ApplyConfiguration(new TimeTrackingEntryConfiguration());
    }
}
