using TaskManagementAPI.Shared.Domain.Interfaces;

namespace TaskManagementAPI.Tests.Unit.Shared.Domain.Interfaces;

/// <summary>
/// Unit tests for INotificationService interface contract.
/// Tests that the interface defines the expected notification operations.
/// </summary>
public class INotificationServiceTests
{
    // Concrete implementation for testing interface
    private class TestNotificationService : INotificationService
    {
        private readonly List<(string UserId, string Message)> _userNotifications = new();
        private readonly List<(string GroupName, string Message)> _broadcastNotifications = new();

        public Task NotifyAsync(string userId, string message)
        {
            _userNotifications.Add((userId, message));
            return Task.CompletedTask;
        }

        public Task BroadcastAsync(string groupName, string message)
        {
            _broadcastNotifications.Add((groupName, message));
            return Task.CompletedTask;
        }

        public int GetUserNotificationCount() => _userNotifications.Count;
        public int GetBroadcastNotificationCount() => _broadcastNotifications.Count;
        public (string UserId, string Message) GetLastUserNotification() => _userNotifications.Last();
        public (string GroupName, string Message) GetLastBroadcastNotification() => _broadcastNotifications.Last();
    }

    [Fact]
    public async Task NotifyAsync_ShouldSendNotificationToUser()
    {
        // Arrange
        var service = new TestNotificationService();
        var userId = "user-123";
        var message = "Test notification";

        // Act
        await service.NotifyAsync(userId, message);

        // Assert
        Assert.Equal(1, service.GetUserNotificationCount());
        var notification = service.GetLastUserNotification();
        Assert.Equal(userId, notification.UserId);
        Assert.Equal(message, notification.Message);
    }

    [Fact]
    public async Task NotifyAsync_ShouldHandleMultipleNotifications()
    {
        // Arrange
        var service = new TestNotificationService();

        // Act
        await service.NotifyAsync("user-1", "Message 1");
        await service.NotifyAsync("user-2", "Message 2");

        // Assert
        Assert.Equal(2, service.GetUserNotificationCount());
    }

    [Fact]
    public async Task BroadcastAsync_ShouldBroadcastToGroup()
    {
        // Arrange
        var service = new TestNotificationService();
        var groupName = "project-123";
        var message = "Project update";

        // Act
        await service.BroadcastAsync(groupName, message);

        // Assert
        Assert.Equal(1, service.GetBroadcastNotificationCount());
        var broadcast = service.GetLastBroadcastNotification();
        Assert.Equal(groupName, broadcast.GroupName);
        Assert.Equal(message, broadcast.Message);
    }

    [Fact]
    public async Task BroadcastAsync_ShouldHandleMultipleBroadcasts()
    {
        // Arrange
        var service = new TestNotificationService();

        // Act
        await service.BroadcastAsync("group-1", "Message 1");
        await service.BroadcastAsync("group-2", "Message 2");

        // Assert
        Assert.Equal(2, service.GetBroadcastNotificationCount());
    }

    [Fact]
    public async Task NotifyAsync_ShouldAcceptEmptyMessage()
    {
        // Arrange
        var service = new TestNotificationService();

        // Act
        await service.NotifyAsync("user-123", "");

        // Assert
        Assert.Equal(1, service.GetUserNotificationCount());
    }

    [Fact]
    public async Task BroadcastAsync_ShouldAcceptEmptyMessage()
    {
        // Arrange
        var service = new TestNotificationService();

        // Act
        await service.BroadcastAsync("group-123", "");

        // Assert
        Assert.Equal(1, service.GetBroadcastNotificationCount());
    }

    [Fact]
    public void INotificationService_ShouldDefineNotifyAsyncMethod()
    {
        // Arrange
        var service = new TestNotificationService();

        // Assert
        var method = typeof(INotificationService).GetMethod("NotifyAsync");
        Assert.NotNull(method);
        Assert.True(method.IsAbstract || method.DeclaringType == typeof(INotificationService));
    }

    [Fact]
    public void INotificationService_ShouldDefineBroadcastAsyncMethod()
    {
        // Arrange
        var service = new TestNotificationService();

        // Assert
        var method = typeof(INotificationService).GetMethod("BroadcastAsync");
        Assert.NotNull(method);
        Assert.True(method.IsAbstract || method.DeclaringType == typeof(INotificationService));
    }
}
