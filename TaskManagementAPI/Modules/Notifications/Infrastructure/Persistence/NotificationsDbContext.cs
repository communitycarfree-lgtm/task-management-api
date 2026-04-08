using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Notifications.Domain.Entities;
using TaskManagementAPI.Modules.Notifications.Infrastructure.Persistence.Configurations;
using TaskManagementAPI.Shared.Infrastructure;

namespace TaskManagementAPI.Modules.Notifications.Infrastructure.Persistence;

/// <summary>
/// DbContext for the Notifications module.
/// Manages Notification entities.
/// </summary>
public class NotificationsDbContext : BaseDbContext
{
    /// <summary>
    /// Initializes a new instance of the NotificationsDbContext class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet for notifications.
    /// </summary>
    public DbSet<Notification> Notifications { get; set; } = null!;

    /// <summary>
    /// Configures the model for the Notifications module.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
    }
}
