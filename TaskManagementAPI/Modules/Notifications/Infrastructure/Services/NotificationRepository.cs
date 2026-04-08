using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Notifications.Domain.Entities;
using TaskManagementAPI.Modules.Notifications.Infrastructure.Persistence;
using TaskManagementAPI.Shared.Infrastructure.Repositories;

namespace TaskManagementAPI.Modules.Notifications.Infrastructure.Services;

/// <summary>
/// Repository implementation for Notification entity operations.
/// </summary>
public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    private new readonly NotificationsDbContext _context;

    /// <summary>
    /// Initializes a new instance of the NotificationRepository class.
    /// </summary>
    /// <param name="context">The Notifications DbContext.</param>
    public NotificationRepository(NotificationsDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets notifications for a specific user.
    /// </summary>
    public async System.Threading.Tasks.Task<(IEnumerable<Notification> Notifications, int TotalCount)> GetUserNotificationsAsync(
        string userId, int pageNumber = 1, int pageSize = 20)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt);

        var totalCount = await query.CountAsync();

        var notifications = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (notifications, totalCount);
    }

    /// <summary>
    /// Gets unread notifications for a specific user.
    /// </summary>
    public async System.Threading.Tasks.Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    public async System.Threading.Tasks.Task<bool> MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);
        if (notification == null)
            return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }
}
