namespace TaskManagementAPI.Modules.Notifications.Domain.Enums;

/// <summary>
/// Enumeration of notification statuses.
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    /// Notification has not been read.
    /// </summary>
    Unread = 0,

    /// <summary>
    /// Notification has been read.
    /// </summary>
    Read = 1,

    /// <summary>
    /// Notification has been archived.
    /// </summary>
    Archived = 2
}
