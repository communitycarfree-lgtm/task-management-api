using TaskManagementAPI.Modules.Notifications.Domain.Enums;

namespace TaskManagementAPI.Modules.Notifications.Application.DTOs;

/// <summary>
/// Data transfer object for notification information.
/// </summary>
public class NotificationDto
{
    /// <summary>
    /// The notification's unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The user ID receiving the notification.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The notification type.
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// The notification message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Whether the notification has been read.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// The notification status.
    /// </summary>
    public NotificationStatus Status { get; set; }

    /// <summary>
    /// The date the notification was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date the notification was read.
    /// </summary>
    public DateTime? ReadAt { get; set; }
}
