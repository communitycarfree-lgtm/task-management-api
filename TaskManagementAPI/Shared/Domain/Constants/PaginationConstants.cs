namespace TaskManagementAPI.Shared.Domain.Constants;

/// <summary>
/// Constants for pagination configuration across the application.
/// </summary>
public static class PaginationConstants
{
    /// <summary>
    /// Default page number (1-based indexing).
    /// </summary>
    public const int DefaultPageNumber = 1;

    /// <summary>
    /// Default page size (number of items per page).
    /// </summary>
    public const int DefaultPageSize = 20;

    /// <summary>
    /// Maximum allowed page size to prevent DoS attacks.
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// Minimum allowed page size.
    /// </summary>
    public const int MinPageSize = 1;

    /// <summary>
    /// Minimum allowed page number.
    /// </summary>
    public const int MinPageNumber = 1;
}
