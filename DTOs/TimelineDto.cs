using System.ComponentModel.DataAnnotations;
using EP.API.Entities;

namespace EP.API.DTOs;

public class EventTimelineDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTimelineDto : IValidatableObject
{
    [Required]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = TimelineStatus.Pending;

    public int SortOrder { get; set; } = 0;

    [StringLength(200)]
    public string? AssignedTo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => DtoValidation.ValidateTimeline(Title, DueDate, Status);
}

public class UpdateTimelineDto : IValidatableObject
{
    [Required]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = TimelineStatus.Pending;

    public int SortOrder { get; set; } = 0;

    [StringLength(200)]
    public string? AssignedTo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => DtoValidation.ValidateTimeline(Title, DueDate, Status, CompletedDate);
}

public class UpdateTimelineStatusDto : IValidatableObject
{
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var statusResult = DtoValidation.OneOf(Status, TimelineStatus.All, nameof(Status), "Status");
        if (statusResult is not null) yield return statusResult;
    }
}
