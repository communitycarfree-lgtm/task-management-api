namespace TaskManagementAPI.Modules.Notifications.Application.DTOs;

/// <summary>
/// Response model for paginated notification list.
/// </summary>
public class NotificationListResponse
{
    /// <summary>
    /// The list of notifications.
    /// </summary>
    public IEnumerable<NotificationDto> Data { get; set; } = new List<NotificationDto>();

    /// <summary>
    /// The total count of notifications.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The current page number.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The page size.
    /// </summary>
    public int PageSize { get; set; }
}
