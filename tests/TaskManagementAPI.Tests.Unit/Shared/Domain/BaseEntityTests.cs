using FluentAssertions;
using TaskManagementAPI.Shared.Domain;

namespace TaskManagementAPI.Tests.Unit.Shared.Domain;

/// <summary>
/// Unit tests for BaseEntity functionality.
/// Tests GUID ID generation, timestamps, and soft delete tracking.
/// </summary>
public class BaseEntityTests
{
    private class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void Constructor_ShouldGenerateNewGuid()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var entity = new TestEntity();

        var afterCreation = DateTime.UtcNow;

        // Assert
        entity.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        entity.CreatedAt.Should().BeOnOrBefore(afterCreation);
    }

    [Fact]
    public void Constructor_ShouldInitializeIsDeletedToFalse()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldInitializeDeletedAtToNull()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_ShouldInitializeUpdatedAtToNull()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void SoftDelete_ShouldSetIsDeletedToTrue()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        entity.IsDeleted = true;

        // Assert
        entity.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void SoftDelete_ShouldSetDeletedAtToCurrentTime()
    {
        // Arrange
        var entity = new TestEntity();
        var beforeDelete = DateTime.UtcNow;

        // Act
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;

        var afterDelete = DateTime.UtcNow;

        // Assert
        entity.DeletedAt.Should().NotBeNull();
        entity.DeletedAt.Should().BeOnOrAfter(beforeDelete);
        entity.DeletedAt.Should().BeOnOrBefore(afterDelete);
    }

    [Fact]
    public void Restore_ShouldSetIsDeletedToFalse()
    {
        // Arrange
        var entity = new TestEntity { IsDeleted = true, DeletedAt = DateTime.UtcNow };

        // Act
        entity.IsDeleted = false;
        entity.DeletedAt = null;

        // Assert
        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void UpdatedAt_ShouldBeNullUntilModified()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void UpdatedAt_ShouldBeSetWhenModified()
    {
        // Arrange
        var entity = new TestEntity();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        entity.Name = "Updated";
        entity.UpdatedAt = DateTime.UtcNow;

        var afterUpdate = DateTime.UtcNow;

        // Assert
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
        entity.UpdatedAt.Should().BeOnOrBefore(afterUpdate);
    }

    [Fact]
    public void MultipleEntities_ShouldHaveDifferentIds()
    {
        // Arrange & Act
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        // Assert
        entity1.Id.Should().NotBe(entity2.Id);
    }
}
