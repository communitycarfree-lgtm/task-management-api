using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Modules.Tasks.Domain.Enums;

namespace TaskManagementAPI.Modules.Tasks.Application.DTOs;

/// <summary>
/// Request DTO for creating a new task.
/// Validates all required fields and business rules.
/// </summary>
public class CreateTaskRequest
{
    /// <summary>
    /// The project ID (required).
    /// </summary>
    [Required(ErrorMessage = "Project ID is required.")]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// The task title (required, 3-200 characters).
    /// </summary>
    [Required(ErrorMessage = "Task title is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The task description (optional, max 2000 characters).
    /// </summary>
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; set; }

    /// <summary>
    /// The task priority (default: Medium).
    /// </summary>
    [EnumDataType(typeof(TaskPriority), ErrorMessage = "Invalid task priority.")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    /// <summary>
    /// The task due date (optional, must be in the future).
    /// </summary>
    [DataType(DataType.DateTime)]
    [CustomValidation(typeof(CreateTaskRequest), nameof(ValidateDueDate))]
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Validates that the due date is not in the past.
    /// </summary>
    /// <param name="dueDate">The due date to validate.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>ValidationResult.Success if valid; otherwise a ValidationResult with error message.</returns>
    public static ValidationResult? ValidateDueDate(DateTime? dueDate, ValidationContext context)
    {
        if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow)
        {
            return new ValidationResult("Due date cannot be in the past.");
        }
        return ValidationResult.Success;
    }
}
