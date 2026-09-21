namespace EP.API.Entities;

/// <summary>
/// Categories that classify events (e.g., Expo, Religious, Meeting, etc.)
/// </summary>
public class EventCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
