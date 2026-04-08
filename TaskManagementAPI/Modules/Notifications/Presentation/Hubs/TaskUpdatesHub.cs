using Microsoft.AspNetCore.SignalR;

namespace TaskManagementAPI.Modules.Notifications.Presentation.Hubs;

/// <summary>
/// SignalR hub for real-time task and project updates.
/// </summary>
public class TaskUpdatesHub : Hub
{
    /// <summary>
    /// Called when a client connects to the hub.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Joins a project group for receiving project updates.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task JoinProjectGroup(string projectId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"project-{projectId}");
    }

    /// <summary>
    /// Leaves a project group.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LeaveProjectGroup(string projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project-{projectId}");
    }

    /// <summary>
    /// Sends a task update to all clients in the project group.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="message">The update message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendTaskUpdate(string projectId, string message)
    {
        await Clients.Group($"project-{projectId}").SendAsync("TaskUpdated", message);
    }

    /// <summary>
    /// Sends a project update to all clients in the project group.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="message">The update message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendProjectUpdate(string projectId, string message)
    {
        await Clients.Group($"project-{projectId}").SendAsync("ProjectUpdated", message);
    }

    /// <summary>
    /// Sends a notification to a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="message">The notification message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendNotification(string userId, string message)
    {
        await Clients.Group($"user-{userId}").SendAsync("NotificationReceived", message);
    }
}
