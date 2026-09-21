using System.ComponentModel.DataAnnotations;
using EP.API.Entities;

namespace EP.API.DTOs;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? ExpectedAttendees { get; set; }
    public int? MaxCapacity { get; set; }
    public decimal? TicketPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Objectives { get; set; }
    public string? RiskManagementPlan { get; set; }
    public decimal? Budget { get; set; }
    public string? PermitsNotes { get; set; }
    public string? TransportationNotes { get; set; }
    public string? EquipmentNotes { get; set; }
    public string? CateringNotes { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation data
    public int EventCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? VenueId { get; set; }
    public string? VenueName { get; set; }
    public string? VenueAddress { get; set; }
    public string? VenueCity { get; set; }

    public List<EventSpeakerDto> Speakers { get; set; } = new();
    public List<EventTimelineDto> Timelines { get; set; } = new();
}

public class CreateEventDto : IValidatableObject
{
    [Required]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? ShortDescription { get; set; }

    [StringLength(1000)]
    public string? ImageUrl { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = EventStatus.Draft;

    public int? ExpectedAttendees { get; set; }
    public int? MaxCapacity { get; set; }
    public decimal? TicketPrice { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "USD";

    [StringLength(2000)]
    public string? Objectives { get; set; }

    [StringLength(4000)]
    public string? RiskManagementPlan { get; set; }

    public decimal? Budget { get; set; }

    [StringLength(2000)]
    public string? PermitsNotes { get; set; }

    [StringLength(2000)]
    public string? TransportationNotes { get; set; }

    [StringLength(2000)]
    public string? EquipmentNotes { get; set; }

    [StringLength(2000)]
    public string? CateringNotes { get; set; }

    public int EventCategoryId { get; set; }
    public int? VenueId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => DtoValidation.ValidateEvent(
            Title,
            StartDate,
            EndDate,
            Status,
            ExpectedAttendees,
            MaxCapacity,
            TicketPrice,
            Currency,
            Budget,
            EventCategoryId,
            VenueId);
}

public class UpdateEventDto : IValidatableObject
{
    [Required]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? ShortDescription { get; set; }

    [StringLength(1000)]
    public string? ImageUrl { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = EventStatus.Draft;

    public int? ExpectedAttendees { get; set; }
    public int? MaxCapacity { get; set; }
    public decimal? TicketPrice { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "USD";

    [StringLength(2000)]
    public string? Objectives { get; set; }

    [StringLength(4000)]
    public string? RiskManagementPlan { get; set; }

    public decimal? Budget { get; set; }

    [StringLength(2000)]
    public string? PermitsNotes { get; set; }

    [StringLength(2000)]
    public string? TransportationNotes { get; set; }

    [StringLength(2000)]
    public string? EquipmentNotes { get; set; }

    [StringLength(2000)]
    public string? CateringNotes { get; set; }

    public int EventCategoryId { get; set; }
    public int? VenueId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => DtoValidation.ValidateEvent(
            Title,
            StartDate,
            EndDate,
            Status,
            ExpectedAttendees,
            MaxCapacity,
            TicketPrice,
            Currency,
            Budget,
            EventCategoryId,
            VenueId);
}

public class UpdateEventStatusDto : IValidatableObject
{
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var statusResult = DtoValidation.OneOf(Status, EventStatus.All, nameof(Status), "Status");
        if (statusResult is not null) yield return statusResult;
    }
}
