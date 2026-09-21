namespace EP.API.Entities;

/// <summary>
/// Physical location / venue where events are held.
/// </summary>
public class Venue
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public int? Capacity { get; set; }
    public string? Amenities { get; set; } // JSON array of amenities
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
