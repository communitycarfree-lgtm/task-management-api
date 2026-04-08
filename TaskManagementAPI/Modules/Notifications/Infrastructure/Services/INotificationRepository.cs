using TaskManagementAPI.Modules.Notifications.Domain.Entities;
using TaskManagementAPI.Shared.Domain.Interfaces;

namespace TaskManagementAPI.Modules.Notifications.Infrastructure.Services;

/// <summary>
/// Repository interface for Notification entity operations.
/// </summary>
public interface INotificationRepository : IRepository<Notification>
{
    /// <summary>
    /// Gets notifications for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A tuple containing the notifications and total count.</returns>
    System.Threading.Tasks.Task<(IEnumerable<Notification> Notifications, int TotalCount)> GetUserNotificationsAsync(
        string userId, int pageNumber = 1, int pageSize = 20);

    /// <summary>
    /// Gets unread notifications for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>A collection of unread notifications.</returns>
    System.Threading.Tasks.Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    /// <param name="notificationId">The notification ID.</param>
    /// <returns>True if successful; otherwise false.</returns>
    System.Threading.Tasks.Task<bool> MarkAsReadAsync(Guid notificationId);
}
