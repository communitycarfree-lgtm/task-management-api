using Serilog;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Shared.Infrastructure.Configuration;
using TaskManagementAPI.Shared.Infrastructure.DependencyInjection;
using TaskManagementAPI.Modules.Projects.Configuration;
using TaskManagementAPI.Modules.Tasks.Configuration;
using TaskManagementAPI.Modules.Users.Configuration;
using TaskManagementAPI.Modules.Notifications.Configuration;
using TaskManagementAPI.Modules.Projects.Infrastructure.Persistence;
using TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence;
using TaskManagementAPI.Modules.Users.Infrastructure.Persistence;
using TaskManagementAPI.Modules.Notifications.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings and module-specific files
ConfigurationLoader.LoadConfiguration(builder);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddSharedServices();
builder.Services.AddProjectsModule(builder.Configuration);
builder.Services.AddTasksModule(builder.Configuration);
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddNotificationsModule(builder.Configuration);

var app = builder.Build();

// Initialize databases
try
{
    using (var scope = app.Services.CreateScope())
    {
        var projectsContext = scope.ServiceProvider.GetRequiredService<ProjectsDbContext>();
        var tasksContext = scope.ServiceProvider.GetRequiredService<TasksDbContext>();
        var usersContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        var notificationsContext = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        // Using MigrateAsync to ensure correct tracking via __EFMigrationsHistory
        try
        {
            await projectsContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error migrating ProjectsDbContext");
        }

        try
        {
            await tasksContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error migrating TasksDbContext");
        }

        try
        {
            await usersContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error migrating UsersDbContext");
        }

        try
        {
            await notificationsContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error migrating NotificationsDbContext");
        }
    }
}
catch (Exception ex)
{
    Log.Error(ex, "An error occurred while initializing the database");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
        c.RoutePrefix = "swagger";
    });
}

app.MapControllers();
app.UseSharedMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Configure default files and static files
var defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(defaultFilesOptions);
app.UseStaticFiles();

// Handle 404 errors for non-API routes
app.Use(async (context, next) =>
{
    await next();
    
    // If response is 404 and not an API route, serve 404.html
    if (context.Response.StatusCode == 404 && !context.Request.Path.StartsWithSegments("/api"))
    {
        context.Request.Path = "/404.html";
        await next();
    }
});

app.Run();

public partial class Program { }
