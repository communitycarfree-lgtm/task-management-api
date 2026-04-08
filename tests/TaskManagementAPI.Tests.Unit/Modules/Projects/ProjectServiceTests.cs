using Moq;
using TaskManagementAPI.Modules.Projects.Application.Services;
using TaskManagementAPI.Modules.Projects.Domain.Entities;
using TaskManagementAPI.Modules.Projects.Domain.Enums;
using TaskManagementAPI.Modules.Projects.Infrastructure.Services;
using TaskManagementAPI.Shared.Domain.Interfaces;
using Xunit;

namespace TaskManagementAPI.Tests.Unit.Modules.Projects;

/// <summary>
/// Unit tests for ProjectService.
/// </summary>
public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly ProjectService _projectService;

    public ProjectServiceTests()
    {
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockNotificationService = new Mock<INotificationService>();
        _projectService = new ProjectService(_mockProjectRepository.Object, _mockNotificationService.Object);
    }

    [Fact]
    public async Task CreateProjectAsync_WithValidData_CreatesProject()
    {
        // Arrange
        var name = "Test Project";
        var description = "Test Description";

        _mockProjectRepository
            .Setup(r => r.AddAsync(It.IsAny<Project>()))
            .ReturnsAsync((Project p) => p);

        // Act
        var result = await _projectService.CreateProjectAsync(name, description);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(description, result.Description);
        Assert.Equal(ProjectStatus.Active, result.Status);
        _mockProjectRepository.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);
        _mockNotificationService.Verify(n => n.BroadcastAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateProjectAsync_WithoutDescription_CreatesProjectWithNullDescription()
    {
        // Arrange
        var name = "Test Project";

        _mockProjectRepository
            .Setup(r => r.AddAsync(It.IsAny<Project>()))
            .ReturnsAsync((Project p) => p);

        // Act
        var result = await _projectService.CreateProjectAsync(name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Null(result.Description);
    }

    [Fact]
    public async Task UpdateProjectAsync_WithValidData_UpdatesProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project { Id = projectId, Name = "Old Name", Description = "Old Description" };
        var newName = "New Name";
        var newDescription = "New Description";

        _mockProjectRepository
            .Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync(existingProject);

        _mockProjectRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Project>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _projectService.UpdateProjectAsync(projectId, newName, newDescription);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);
        Assert.Equal(newDescription, result.Description);
        _mockProjectRepository.Verify(r => r.UpdateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProjectAsync_WithNonExistentProject_ReturnsNull()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _mockProjectRepository
            .Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _projectService.UpdateProjectAsync(projectId, "New Name");

        // Assert
        Assert.Null(result);
        _mockProjectRepository.Verify(r => r.UpdateAsync(It.IsAny<Project>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProjectAsync_WithValidProject_DeletesProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };

        _mockProjectRepository
            .Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        _mockProjectRepository
            .Setup(r => r.DeleteAsync(projectId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _projectService.DeleteProjectAsync(projectId);

        // Assert
        Assert.True(result);
        _mockProjectRepository.Verify(r => r.DeleteAsync(projectId), Times.Once);
    }

    [Fact]
    public async Task DeleteProjectAsync_WithNonExistentProject_ReturnsFalse()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _mockProjectRepository
            .Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _projectService.DeleteProjectAsync(projectId);

        // Assert
        Assert.False(result);
        _mockProjectRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task AddMemberAsync_WithValidData_AddsMember()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = "user-123";
        var project = new Project { Id = projectId, Name = "Test Project", Members = new List<ProjectMember>() };

        _mockProjectRepository
            .Setup(r => r.GetByIdWithMembersAsync(projectId))
            .ReturnsAsync(project);

        _mockProjectRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Project>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _projectService.AddMemberAsync(projectId, userId, ProjectMemberRole.Developer);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(ProjectMemberRole.Developer, result.Role);
        Assert.Single(project.Members);
    }

    [Fact]
    public async Task AddMemberAsync_WithNonExistentProject_ThrowsException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = "user-123";

        _mockProjectRepository
            .Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _projectService.AddMemberAsync(projectId, userId));
    }

    [Fact]
    public async Task RemoveMemberAsync_WithValidMember_RemovesMember()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = "user-123";
        var member = new ProjectMember { Id = Guid.NewGuid(), ProjectId = projectId, UserId = userId };
        var project = new Project { Id = projectId, Name = "Test Project", Members = new List<ProjectMember> { member } };

        _mockProjectRepository
            .Setup(r => r.GetByIdWithMembersAsync(projectId))
            .ReturnsAsync(project);

        _mockProjectRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Project>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _projectService.RemoveMemberAsync(projectId, userId);

        // Assert
        Assert.True(result);
        Assert.Empty(project.Members);
    }

    [Fact]
    public async Task RemoveMemberAsync_WithNonExistentProject_ReturnsFalse()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = "user-123";

        _mockProjectRepository
            .Setup(r => r.GetByIdWithMembersAsync(projectId))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _projectService.RemoveMemberAsync(projectId, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveMemberAsync_WithNonExistentMember_ReturnsFalse()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = "user-123";
        var project = new Project { Id = projectId, Name = "Test Project", Members = new List<ProjectMember>() };

        _mockProjectRepository
            .Setup(r => r.GetByIdWithMembersAsync(projectId))
            .ReturnsAsync(project);

        // Act
        var result = await _projectService.RemoveMemberAsync(projectId, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetProjectWithMembersAsync_WithValidProject_ReturnsProjectWithMembers()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project", Members = new List<ProjectMember>() };

        _mockProjectRepository
            .Setup(r => r.GetByIdWithMembersAsync(projectId))
            .ReturnsAsync(project);

        // Act
        var result = await _projectService.GetProjectWithMembersAsync(projectId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(projectId, result.Id);
    }

    [Fact]
    public async Task GetUserProjectsAsync_WithValidUser_ReturnsUserProjects()
    {
        // Arrange
        var userId = "user-123";
        var projects = new List<Project>
        {
            new Project { Id = Guid.NewGuid(), Name = "Project 1" },
            new Project { Id = Guid.NewGuid(), Name = "Project 2" }
        };

        _mockProjectRepository
            .Setup(r => r.GetUserProjectsAsync(userId))
            .ReturnsAsync(projects);

        // Act
        var result = await _projectService.GetUserProjectsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetProjectsPagedAsync_WithValidParameters_ReturnsPaginatedProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project { Id = Guid.NewGuid(), Name = "Project 1" },
            new Project { Id = Guid.NewGuid(), Name = "Project 2" }
        };

        _mockProjectRepository
            .Setup(r => r.GetProjectsPagedAsync(1, 20))
            .ReturnsAsync((projects, 2));

        // Act
        var (result, totalCount) = await _projectService.GetProjectsPagedAsync(1, 20);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(2, totalCount);
    }
}
