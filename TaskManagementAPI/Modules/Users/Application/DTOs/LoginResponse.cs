namespace TaskManagementAPI.Modules.Users.Application.DTOs;

/// <summary>
/// Response model for user login containing JWT token.
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// The JWT access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The token type (Bearer).
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// The token expiration time in seconds.
    /// </summary>
    public int ExpiresIn { get; set; } = 3600;

    /// <summary>
    /// The authenticated user information.
    /// </summary>
    public UserDto? User { get; set; }
}
