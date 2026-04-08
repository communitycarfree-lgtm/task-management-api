namespace TaskManagementAPI.Shared.Domain.Interfaces;

/// <summary>
/// Notification service interface for real-time updates.
/// Provides a contract for sending notifications to users and broadcasting to groups.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a notification to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to notify.</param>
    /// <param name="message">The notification message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task NotifyAsync(string userId, string message);

    /// <summary>
    /// Broadcasts a notification to all users in a group.
    /// </summary>
    /// <param name="groupName">The name of the group to broadcast to.</param>
    /// <param name="message">The notification message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BroadcastAsync(string groupName, string message);
}
