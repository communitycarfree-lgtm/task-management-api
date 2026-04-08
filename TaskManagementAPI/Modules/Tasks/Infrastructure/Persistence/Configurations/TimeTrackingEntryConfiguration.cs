using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementAPI.Modules.Tasks.Domain.Entities;

namespace TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the TimeTrackingEntry entity.
/// </summary>
public class TimeTrackingEntryConfiguration : IEntityTypeConfiguration<TimeTrackingEntry>
{
    /// <summary>
    /// Configures the TimeTrackingEntry entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<TimeTrackingEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.TaskId)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(e => e.Hours)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(e => e.Date)
            .IsRequired();

        // Indexes for common queries
        builder.HasIndex(e => e.TaskId);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Date);

        // Relationships
        builder.HasOne(e => e.Task)
            .WithMany(t => t.TimeTrackingEntries)
            .HasForeignKey(e => e.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
