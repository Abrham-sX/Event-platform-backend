namespace EP.API.Entities;

/// <summary>
/// Core Event entity — the main object in the platform.
/// </summary>
public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }

    /// <summary>URL to the event banner/image.</summary>
    public string? ImageUrl { get; set; }

    /// <summary>Start date and time of the event.</summary>
    public DateTime StartDate { get; set; }

    /// <summary>End date and time of the event.</summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Event status: Draft, Published, Cancelled, Completed, Postponed
    /// </summary>
    public string Status { get; set; } = EventStatus.Draft;

    /// <summary>Expected number of attendees.</summary>
    public int? ExpectedAttendees { get; set; }

    /// <summary>Maximum capacity for the event.</summary>
    public int? MaxCapacity { get; set; }

    /// <summary>Ticket price in the default currency. Null if free.</summary>
    public decimal? TicketPrice { get; set; }

    /// <summary>Currency code (e.g., USD, EUR, KES).</summary>
    public string Currency { get; set; } = "USD";

    /// <summary>Event objectives / goals.</summary>
    public string? Objectives { get; set; }

    /// <summary>Risk assessment and contingency plans.</summary>
    public string? RiskManagementPlan { get; set; }

    /// <summary>Budget allocated for this event.</summary>
    public decimal? Budget { get; set; }

    /// <summary>Notes about permits and legal requirements.</summary>
    public string? PermitsNotes { get; set; }

    /// <summary>Transportation coordination details.</summary>
    public string? TransportationNotes { get; set; }

    /// <summary>Equipment and facilities needed.</summary>
    public string? EquipmentNotes { get; set; }

    /// <summary>Catering details.</summary>
    public string? CateringNotes { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign Keys
    public int EventCategoryId { get; set; }
    public int? VenueId { get; set; }

    // Navigation Properties
    public EventCategory Category { get; set; } = null!;
    public Venue? Venue { get; set; }
    public ICollection<EventSpeaker> EventSpeakers { get; set; } = new List<EventSpeaker>();
    public ICollection<EventTimeline> Timelines { get; set; } = new List<EventTimeline>();
}

/// <summary>
/// Constants for event status values.
/// </summary>
public static class EventStatus
{
    public const string Draft = "Draft";
    public const string Published = "Published";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
    public const string Postponed = "Postponed";

    public static readonly string[] All = { Draft, Published, InProgress, Completed, Cancelled, Postponed };
}
