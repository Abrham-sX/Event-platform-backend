namespace EP.API.Entities;

/// <summary>
/// Join table linking Events to Speakers with role and order.
/// </summary>
public class EventSpeaker
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int SpeakerId { get; set; }
    public string? Role { get; set; } // e.g., Keynote, Panelist, Host
    public int SortOrder { get; set; } = 0;

    // Navigation
    public Event Event { get; set; } = null!;
    public Speaker Speaker { get; set; } = null!;
}
