using TaskManagementAPI.Modules.Notifications.Domain.Entities;
using TaskManagementAPI.Modules.Notifications.Domain.Enums;
using TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Seeds;

namespace TaskManagementAPI.Modules.Notifications.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeder for initial notification data.
/// </summary>
public class NotificationSeeder : ISeeder
{
    /// <summary>
    /// Seeds initial notifications into the database.
    /// </summary>
    public async System.Threading.Tasks.Task SeedAsync(Microsoft.EntityFrameworkCore.DbContext context)
    {
        var notificationsDbContext = context as NotificationsDbContext;
        if (notificationsDbContext == null)
            return;

        // Check if notifications already exist
        if (notificationsDbContext.Notifications.Any())
            return;

        var notifications = new[]
        {
            new Notification
            {
                UserId = "user1",
                Type = NotificationType.TaskAssigned,
                Message = "You have been assigned to a new task",
                Status = NotificationStatus.Unread,
                IsRead = false
            },
            new Notification
            {
                UserId = "user2",
                Type = NotificationType.DueDateApproaching,
                Message = "Your task is due in 24 hours",
                Status = NotificationStatus.Unread,
                IsRead = false
            }
        };

        await notificationsDbContext.Notifications.AddRangeAsync(notifications);
        await notificationsDbContext.SaveChangesAsync();
    }
}
