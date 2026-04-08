namespace TaskManagementAPI.Shared.Domain.Constants;

/// <summary>
/// Constants for validation rules across the application.
/// </summary>
public static class ValidationConstants
{
    /// <summary>
    /// Minimum length for string fields like titles.
    /// </summary>
    public const int MinStringLength = 3;

    /// <summary>
    /// Maximum length for task and project titles.
    /// </summary>
    public const int MaxTitleLength = 200;

    /// <summary>
    /// Maximum length for descriptions.
    /// </summary>
    public const int MaxDescriptionLength = 2000;

    /// <summary>
    /// Minimum password length for security compliance.
    /// </summary>
    public const int MinPasswordLength = 12;

    /// <summary>
    /// Maximum password length to prevent buffer overflow attacks.
    /// </summary>
    public const int MaxPasswordLength = 128;

    /// <summary>
    /// Minimum length for user full names.
    /// </summary>
    public const int MinFullNameLength = 2;

    /// <summary>
    /// Maximum length for user full names.
    /// </summary>
    public const int MaxFullNameLength = 100;
}
