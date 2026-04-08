using Microsoft.Extensions.Configuration;

namespace TaskManagementAPI.Shared.Infrastructure.Configuration;

/// <summary>
/// Validates that all required configuration values are present at startup.
/// Fails fast if critical configuration is missing.
/// </summary>
public static class ConfigurationValidator
{
    /// <summary>
    /// Validates that all required configuration sections exist.
    /// Throws an exception if any required configuration is missing.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <exception cref="InvalidOperationException">Thrown when required configuration is missing.</exception>
    public static void ValidateConfiguration(IConfiguration configuration)
    {
        var missingConfigs = new List<string>();

        // Validate database connection strings
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            missingConfigs.Add("ConnectionStrings:DefaultConnection");
        }

        // Validate JWT settings
        var jwtSection = configuration.GetSection("Jwt");
        if (!jwtSection.Exists())
        {
            missingConfigs.Add("Jwt section");
        }
        else
        {
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            if (string.IsNullOrWhiteSpace(key))
                missingConfigs.Add("Jwt:Key");
            if (string.IsNullOrWhiteSpace(issuer))
                missingConfigs.Add("Jwt:Issuer");
            if (string.IsNullOrWhiteSpace(audience))
                missingConfigs.Add("Jwt:Audience");
        }

        // Validate Serilog settings
        var serilogSection = configuration.GetSection("Serilog");
        if (!serilogSection.Exists())
        {
            missingConfigs.Add("Serilog section");
        }

        if (missingConfigs.Count > 0)
        {
            var message = $"Missing required configuration values:\n{string.Join("\n", missingConfigs)}";
            throw new InvalidOperationException(message);
        }
    }
}
