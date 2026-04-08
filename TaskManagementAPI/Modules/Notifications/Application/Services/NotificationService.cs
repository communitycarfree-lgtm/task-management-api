using TaskManagementAPI.Modules.Notifications.Domain.Entities;
using TaskManagementAPI.Modules.Notifications.Domain.Enums;
using TaskManagementAPI.Modules.Notifications.Infrastructure.Services;

namespace TaskManagementAPI.Modules.Notifications.Application.Services;

/// <summary>
/// Service for managing notifications.
/// </summary>
public class NotificationService
{
    private readonly INotificationRepository _notificationRepository;

    /// <summary>
    /// Initializes a new instance of the NotificationService class.
    /// </summary>
    /// <param name="notificationRepository">The notification repository.</param>
    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    /// <summary>
    /// Creates a new notification.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="type">The notification type.</param>
    /// <param name="message">The notification message.</param>
    /// <returns>The created notification.</returns>
    public async System.Threading.Tasks.Task<Notification> CreateNotificationAsync(
        string userId, NotificationType type, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Message = message,
            Status = NotificationStatus.Unread,
            IsRead = false
        };

        await _notificationRepository.AddAsync(notification);
        return notification;
    }

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    /// <param name="notificationId">The notification ID.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public async System.Threading.Tasks.Task<bool> MarkAsReadAsync(Guid notificationId)
    {
        return await _notificationRepository.MarkAsReadAsync(notificationId);
    }

    /// <summary>
    /// Deletes a notification (soft delete).
    /// </summary>
    /// <param name="notificationId">The notification ID.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public async System.Threading.Tasks.Task<bool> DeleteNotificationAsync(Guid notificationId)
    {
        await _notificationRepository.DeleteAsync(notificationId);
        return true;
    }

    /// <summary>
    /// Gets notifications for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A tuple containing the notifications and total count.</returns>
    public async System.Threading.Tasks.Task<(IEnumerable<Notification> Notifications, int TotalCount)> GetUserNotificationsAsync(
        string userId, int pageNumber = 1, int pageSize = 20)
    {
        return await _notificationRepository.GetUserNotificationsAsync(userId, pageNumber, pageSize);
    }

    /// <summary>
    /// Gets unread notifications for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>A collection of unread notifications.</returns>
    public async System.Threading.Tasks.Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
    {
        return await _notificationRepository.GetUnreadNotificationsAsync(userId);
    }
}
