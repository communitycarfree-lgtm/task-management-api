using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementAPI.Modules.Projects.Domain.Entities;

namespace TaskManagementAPI.Modules.Projects.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the Project entity using Fluent API.
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    /// <summary>
    /// Configures the Project entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.DeletedAt);

        // Relationships
        builder.HasMany(p => p.Members)
            .WithOne(pm => pm.Project)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.IsDeleted);
        builder.HasIndex(p => new { p.IsDeleted, p.CreatedAt });

        // Query filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);

        // Table name
        builder.ToTable("Projects");
    }
}
