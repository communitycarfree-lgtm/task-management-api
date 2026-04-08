using FluentAssertions;
using Microsoft.Extensions.Configuration;
using TaskManagementAPI.Shared.Infrastructure.Configuration;

namespace TaskManagementAPI.Tests.Unit.Shared.Infrastructure;

/// <summary>
/// Unit tests for configuration loading and validation.
/// Tests that required configuration values are validated at startup.
/// </summary>
public class ConfigurationTests
{
    private IConfigurationBuilder CreateConfigurationBuilder()
    {
        return new ConfigurationBuilder();
    }

    [Fact]
    public void ValidateConfiguration_WithAllRequiredSettings_ShouldSucceed()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDB;" },
                { "Jwt:Key", "test-key-minimum-32-characters-long" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Serilog:MinimumLevel", "Information" }
            })
            .Build();

        // Act & Assert
        var action = () => ConfigurationValidator.ValidateConfiguration(config);
        action.Should().NotThrow();
    }

    [Fact]
    public void ValidateConfiguration_WithMissingConnectionString_ShouldThrow()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "test-key-minimum-32-characters-long" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Serilog:MinimumLevel", "Information" }
            })
            .Build();

        // Act & Assert
        var action = () => ConfigurationValidator.ValidateConfiguration(config);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*ConnectionStrings:DefaultConnection*");
    }

    [Fact]
    public void ValidateConfiguration_WithMissingJwtKey_ShouldThrow()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDB;" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Serilog:MinimumLevel", "Information" }
            })
            .Build();

        // Act & Assert
        var action = () => ConfigurationValidator.ValidateConfiguration(config);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Jwt:Key*");
    }

    [Fact]
    public void ValidateConfiguration_WithMissingJwtIssuer_ShouldThrow()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDB;" },
                { "Jwt:Key", "test-key-minimum-32-characters-long" },
                { "Jwt:Audience", "TestAudience" },
                { "Serilog:MinimumLevel", "Information" }
            })
            .Build();

        // Act & Assert
        var action = () => ConfigurationValidator.ValidateConfiguration(config);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Jwt:Issuer*");
    }

    [Fact]
    public void ValidateConfiguration_WithMissingJwtAudience_ShouldThrow()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDB;" },
                { "Jwt:Key", "test-key-minimum-32-characters-long" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Serilog:MinimumLevel", "Information" }
            })
            .Build();

        // Act & Assert
        var action = () => ConfigurationValidator.ValidateConfiguration(config);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Jwt:Audience*");
    }

    [Fact]
    public void ValidateConfiguration_WithMissingSerilogSection_ShouldThrow()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDB;" },
                { "Jwt:Key", "test-key-minimum-32-characters-long" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            })
            .Build();

        // Act & Assert
        var action = () => ConfigurationValidator.ValidateConfiguration(config);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Serilog*");
    }

    [Fact]
    public void ValidateConfiguration_WithMultipleMissingSettings_ShouldThrowWithAllMissing()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act & Assert
        var action = () => ConfigurationValidator.ValidateConfiguration(config);
        var exception = action.Should().Throw<InvalidOperationException>().Which;
        exception.Message.Should().Contain("ConnectionStrings:DefaultConnection");
        exception.Message.Should().Contain("Jwt section");
        exception.Message.Should().Contain("Serilog section");
    }

    [Fact]
    public void ValidateConfiguration_WithEmptyConnectionString_ShouldThrow()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "" },
                { "Jwt:Key", "test-key-minimum-32-characters-long" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Serilog:MinimumLevel", "Information" }
            })
            .Build();

        // Act & Assert
        var action = () => ConfigurationValidator.ValidateConfiguration(config);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*ConnectionStrings:DefaultConnection*");
    }

    [Fact]
    public void ValidateConfiguration_WithWhitespaceConnectionString_ShouldThrow()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "   " },
                { "Jwt:Key", "test-key-minimum-32-characters-long" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Serilog:MinimumLevel", "Information" }
            })
            .Build();

        // Act & Assert
        var action = () => ConfigurationValidator.ValidateConfiguration(config);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*ConnectionStrings:DefaultConnection*");
    }
}
