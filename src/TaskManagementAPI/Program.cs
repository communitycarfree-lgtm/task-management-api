using Serilog;
using TaskManagementAPI.Shared.Infrastructure.Configuration;
using TaskManagementAPI.Shared.Infrastructure.DependencyInjection;
using TaskManagementAPI.Modules.Projects.Configuration;
using TaskManagementAPI.Modules.Tasks.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings and module-specific files
ConfigurationLoader.LoadConfiguration(builder);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddSharedServices();
builder.Services.AddProjectsModule(builder.Configuration);
builder.Services.AddTasksModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSharedMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
