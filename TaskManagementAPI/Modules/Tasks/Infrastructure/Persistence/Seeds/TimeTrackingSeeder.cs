using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Tasks.Domain.Entities;

namespace TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeder for time tracking entry data.
/// </summary>
public class TimeTrackingSeeder : ISeeder
{
    /// <summary>
    /// Seeds initial time tracking entries into the database.
    /// </summary>
    /// <param name="context">The DbContext.</param>
    public async Task SeedAsync(DbContext context)
    {
        var tasksContext = context as TasksDbContext;
        if (tasksContext == null)
            return;

        // Check if time tracking entries already exist
        if (await tasksContext.TimeTrackingEntries.AnyAsync())
            return;

        // Create sample time tracking entries
        var entries = new List<TimeTrackingEntry>
        {
            new TimeTrackingEntry
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333331"),
                TaskId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // "Design database schema"
                UserId = "user1",
                Hours = 8,
                Date = DateTime.UtcNow.AddDays(-10),
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new TimeTrackingEntry
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333332"),
                TaskId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // "Design database schema"
                UserId = "user1",
                Hours = 6,
                Date = DateTime.UtcNow.AddDays(-9),
                CreatedAt = DateTime.UtcNow.AddDays(-9)
            },
            new TimeTrackingEntry
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                TaskId = Guid.Parse("11111111-1111-1111-1111-111111111112"), // "Implement API endpoints"
                UserId = "user2",
                Hours = 4,
                Date = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new TimeTrackingEntry
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333334"),
                TaskId = Guid.Parse("11111111-1111-1111-1111-111111111112"), // "Implement API endpoints"
                UserId = "user2",
                Hours = 6,
                Date = DateTime.UtcNow.AddDays(-4),
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            }
        };

        await tasksContext.TimeTrackingEntries.AddRangeAsync(entries);
        await tasksContext.SaveChangesAsync();
    }
}
