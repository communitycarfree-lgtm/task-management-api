using Microsoft.Extensions.Configuration;

namespace TaskManagementAPI.Shared.Infrastructure.Configuration;

/// <summary>
/// Loads configuration from appsettings files and module-specific configuration files.
/// Supports environment-specific overrides and environment variable substitution.
/// </summary>
public static class ConfigurationLoader
{
    /// <summary>
    /// Loads configuration from appsettings.json, environment-specific files, and module configurations.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    /// <param name="modulesConfigPath">The path to the modules configuration directory.</param>
    public static void LoadConfiguration(WebApplicationBuilder builder, string modulesConfigPath = "Configuration")
    {
        // Load base configuration
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Load module-specific configurations
        LoadModuleConfigurations(builder.Configuration, modulesConfigPath);

        // Validate configuration
        ConfigurationValidator.ValidateConfiguration(builder.Configuration);
    }

    /// <summary>
    /// Loads all module-specific configuration files from the specified directory.
    /// </summary>
    /// <param name="configuration">The configuration builder.</param>
    /// <param name="modulesConfigPath">The path to the modules configuration directory.</param>
    private static void LoadModuleConfigurations(IConfigurationBuilder configuration, string modulesConfigPath)
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, modulesConfigPath);

        if (!Directory.Exists(configPath))
        {
            return; // No module configurations to load
        }

        var configFiles = Directory.GetFiles(configPath, "*-config.json");

        foreach (var configFile in configFiles)
        {
            configuration.AddJsonFile(configFile, optional: false, reloadOnChange: true);
        }
    }
}
