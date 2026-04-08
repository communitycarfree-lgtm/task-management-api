using TaskManagementAPI.Modules.Notifications.Domain.Enums;
using TaskManagementAPI.Shared.Domain;

namespace TaskManagementAPI.Modules.Notifications.Domain.Entities;

/// <summary>
/// Represents a notification sent to a user.
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// The ID of the user receiving the notification.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The type of notification.
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// The notification message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Whether the notification has been read.
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// The status of the notification.
    /// </summary>
    public NotificationStatus Status { get; set; } = NotificationStatus.Unread;

    /// <summary>
    /// The date the notification was read.
    /// </summary>
    public DateTime? ReadAt { get; set; }
}
