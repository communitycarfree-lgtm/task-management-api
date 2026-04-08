using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementAPI.Modules.Tasks.Domain.Entities;

namespace TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the WorkTask entity.
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<WorkTask>
{
    /// <summary>
    /// Configures the WorkTask entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<WorkTask> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.ProjectId)
            .IsRequired();

        builder.Property(t => t.AssigneeId)
            .HasMaxLength(450);

        builder.Property(t => t.DueDate);

        // Indexes for common queries
        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.AssigneeId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.CreatedAt);
        builder.HasIndex(t => t.DueDate);

        // Relationships
        builder.HasMany(t => t.BlockedByDependencies)
            .WithOne(d => d.Task)
            .HasForeignKey(d => d.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.BlockingDependencies)
            .WithOne(d => d.BlockedByTask)
            .HasForeignKey(d => d.BlockedByTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.TimeTrackingEntries)
            .WithOne(e => e.Task)
            .HasForeignKey(e => e.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
