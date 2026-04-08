using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Projects.Application.DTOs;
using TaskManagementAPI.Modules.Projects.Application.Services;
using TaskManagementAPI.Modules.Projects.Domain.Entities;
using TaskManagementAPI.Modules.Projects.Domain.Enums;
using TaskManagementAPI.Modules.Projects.Infrastructure.Persistence;
using TaskManagementAPI.Modules.Projects.Infrastructure.Services;
using TaskManagementAPI.Shared.Domain.Interfaces;
using Xunit;
using Moq;

namespace TaskManagementAPI.Tests.Integration.Modules.Projects;

/// <summary>
/// Integration tests for Projects module.
/// </summary>
public class ProjectsControllerIntegrationTests : IAsyncLifetime
{
    private ProjectsDbContext _context = null!;
    private ProjectService _projectService = null!;
    private IProjectRepository _projectRepository = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ProjectsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ProjectsDbContext(options);
        await _context.Database.EnsureCreatedAsync();

        _projectRepository = new ProjectRepository(_context);
        var mockNotificationService = new Mock<INotificationService>();
        _projectService = new ProjectService(_projectRepository, mockNotificationService.Object);
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task CreateProject_WithValidData_CreatesProjectInDatabase()
    {
        // Arrange
        var name = "Integration Test Project";
        var description = "Test Description";

        // Act
        var project = await _projectService.CreateProjectAsync(name, description);
        await _context.SaveChangesAsync();

        // Assert
        var savedProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        Assert.NotNull(savedProject);
        Assert.Equal(name, savedProject.Name);
        Assert.Equal(description, savedProject.Description);
        Assert.Equal(ProjectStatus.Active, savedProject.Status);
    }

    [Fact]
    public async Task GetProjectWithMembers_ReturnsProjectWithAllMembers()
    {
        // Arrange
        var project = new Project { Name = "Test Project", Description = "Test" };
        var member1 = new ProjectMember { ProjectId = project.Id, UserId = "user-1", Role = ProjectMemberRole.Owner };
        var member2 = new ProjectMember { ProjectId = project.Id, UserId = "user-2", Role = ProjectMemberRole.Developer };

        project.Members.Add(member1);
        project.Members.Add(member2);

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _projectService.GetProjectWithMembersAsync(project.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Members.Count);
        Assert.Contains(result.Members, m => m.UserId == "user-1");
        Assert.Contains(result.Members, m => m.UserId == "user-2");
    }

    [Fact]
    public async Task UpdateProject_UpdatesProjectInDatabase()
    {
        // Arrange
        var project = new Project { Name = "Original Name", Description = "Original Description" };
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        var newName = "Updated Name";
        var newDescription = "Updated Description";

        // Act
        var updated = await _projectService.UpdateProjectAsync(project.Id, newName, newDescription);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(updated);
        var savedProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        Assert.NotNull(savedProject);
        Assert.Equal(newName, savedProject.Name);
        Assert.Equal(newDescription, savedProject.Description);
    }

    [Fact]
    public async Task DeleteProject_SoftDeletesProject()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        // Act
        await _projectService.DeleteProjectAsync(project.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deletedProject = await _context.Projects.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == project.Id);
        Assert.NotNull(deletedProject);
        Assert.True(deletedProject.IsDeleted);
        Assert.NotNull(deletedProject.DeletedAt);

        // Verify soft delete filter works
        var activeProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        Assert.Null(activeProject);
    }

    [Fact]
    public async Task AddMember_AddsProjectMemberToDatabase()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        // Act - Add member directly to test the database behavior
        var member = new ProjectMember { ProjectId = project.Id, UserId = "user-123", Role = ProjectMemberRole.Developer };
        await _context.ProjectMembers.AddAsync(member);
        await _context.SaveChangesAsync();

        // Assert
        var savedMember = await _context.ProjectMembers.FirstOrDefaultAsync(m => m.UserId == "user-123");
        Assert.NotNull(savedMember);
        Assert.Equal("user-123", savedMember.UserId);
        Assert.Equal(ProjectMemberRole.Developer, savedMember.Role);
    }

    [Fact]
    public async Task RemoveMember_RemovesProjectMemberFromDatabase()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        var member = new ProjectMember { ProjectId = project.Id, UserId = "user-123", Role = ProjectMemberRole.Developer };
        project.Members.Add(member);

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        // Act
        var success = await _projectService.RemoveMemberAsync(project.Id, "user-123");
        await _context.SaveChangesAsync();

        // Assert
        Assert.True(success);
        var remainingMembers = await _context.ProjectMembers.Where(m => m.ProjectId == project.Id).ToListAsync();
        Assert.Empty(remainingMembers);
    }

    [Fact]
    public async Task GetUserProjects_ReturnsOnlyUserProjects()
    {
        // Arrange
        var project1 = new Project { Name = "Project 1" };
        var project2 = new Project { Name = "Project 2" };
        var project3 = new Project { Name = "Project 3" };

        var member1 = new ProjectMember { ProjectId = project1.Id, UserId = "user-1", Role = ProjectMemberRole.Owner };
        var member2 = new ProjectMember { ProjectId = project2.Id, UserId = "user-1", Role = ProjectMemberRole.Developer };
        var member3 = new ProjectMember { ProjectId = project3.Id, UserId = "user-2", Role = ProjectMemberRole.Owner };

        project1.Members.Add(member1);
        project2.Members.Add(member2);
        project3.Members.Add(member3);

        await _context.Projects.AddRangeAsync(project1, project2, project3);
        await _context.SaveChangesAsync();

        // Act
        var userProjects = await _projectService.GetUserProjectsAsync("user-1");

        // Assert
        Assert.Equal(2, userProjects.Count());
        Assert.Contains(userProjects, p => p.Id == project1.Id);
        Assert.Contains(userProjects, p => p.Id == project2.Id);
        Assert.DoesNotContain(userProjects, p => p.Id == project3.Id);
    }

    [Fact]
    public async Task GetProjectsPaged_ReturnsPaginatedResults()
    {
        // Arrange
        var projects = Enumerable.Range(1, 25)
            .Select(i => new Project { Name = $"Project {i}" })
            .ToList();

        await _context.Projects.AddRangeAsync(projects);
        await _context.SaveChangesAsync();

        // Act
        var (page1, total1) = await _projectService.GetProjectsPagedAsync(1, 10);
        var (page2, total2) = await _projectService.GetProjectsPagedAsync(2, 10);
        var (page3, total3) = await _projectService.GetProjectsPagedAsync(3, 10);

        // Assert
        Assert.Equal(10, page1.Count());
        Assert.Equal(10, page2.Count());
        Assert.Equal(5, page3.Count());
        Assert.Equal(25, total1);
        Assert.Equal(25, total2);
        Assert.Equal(25, total3);
    }

    [Fact]
    public async Task CascadeDelete_DeletesProjectMembers()
    {
        // Arrange
        var project = new Project { Name = "Test Project" };
        var member = new ProjectMember { ProjectId = project.Id, UserId = "user-1", Role = ProjectMemberRole.Owner };
        project.Members.Add(member);

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        // Act
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        // Assert
        var remainingMembers = await _context.ProjectMembers.Where(m => m.ProjectId == project.Id).ToListAsync();
        Assert.Empty(remainingMembers);
    }
}
