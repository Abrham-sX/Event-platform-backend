namespace EP.API.Entities;

/// <summary>
/// Speakers / presenters associated with events.
/// </summary>
public class Speaker
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? Title { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<EventSpeaker> EventSpeakers { get; set; } = new List<EventSpeaker>();
}
