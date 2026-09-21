namespace EP.API.Entities;

/// <summary>
/// Timeline items / milestones for an event (e.g., "Venue booking confirmed", "Caterer hired").
/// </summary>
public class EventTimeline
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    /// <summary>Pending, InProgress, Completed, Overdue</summary>
    public string Status { get; set; } = TimelineStatus.Pending;
    public int SortOrder { get; set; } = 0;
    public string? AssignedTo { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Event Event { get; set; } = null!;
}

public static class TimelineStatus
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Overdue = "Overdue";

    public static readonly string[] All = { Pending, InProgress, Completed, Overdue };
}
