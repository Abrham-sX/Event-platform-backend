using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EP.API.Data;
using EP.API.DTOs;
using EP.API.Entities;
using EP.API.Repositories;

namespace EP.API.Services;

public class EventService : IEventService
{
    private readonly IRepository<Event> _repo;
    private readonly IRepository<EventCategory> _categoryRepo;
    private readonly IRepository<Venue> _venueRepo;
    private readonly IRepository<Speaker> _speakerRepo;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EventService(
        IRepository<Event> repo,
        IRepository<EventCategory> categoryRepo,
        IRepository<Venue> venueRepo,
        IRepository<Speaker> speakerRepo,
        AppDbContext context,
        IMapper mapper)
    {
        _repo = repo;
        _categoryRepo = categoryRepo;
        _venueRepo = venueRepo;
        _speakerRepo = speakerRepo;
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EventDto>> GetAllAsync()
    {
        var events = await EventDetailsQuery()
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();

        return _mapper.Map<List<EventDto>>(events);
    }

    public async Task<EventDto?> GetByIdAsync(int id)
    {
        var evt = await EventDetailsQuery()
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

        return evt is null ? null : _mapper.Map<EventDto>(evt);
    }

    public async Task<IReadOnlyList<EventDto>> GetByCategoryAsync(int categoryId)
    {
        if (categoryId <= 0)
        {
            throw new ArgumentException("Category ID must be greater than zero.");
        }

        var events = await EventDetailsQuery()
            .Where(e => e.IsActive && e.EventCategoryId == categoryId)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();

        return _mapper.Map<List<EventDto>>(events);
    }

    public async Task<IReadOnlyList<EventDto>> GetByStatusAsync(string status)
    {
        var normalizedStatus = NormalizeEventStatus(status);

        var events = await EventDetailsQuery()
            .Where(e => e.IsActive && e.Status == normalizedStatus)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();

        return _mapper.Map<List<EventDto>>(events);
    }

    public async Task<IReadOnlyList<EventDto>> GetUpcomingAsync(int count = 10)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.");
        }

        var events = await EventDetailsQuery()
            .Where(e => e.IsActive && e.Status == EventStatus.Published && e.StartDate > DateTime.UtcNow)
            .OrderBy(e => e.StartDate)
            .Take(Math.Min(count, 100))
            .ToListAsync();

        return _mapper.Map<List<EventDto>>(events);
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto)
    {
        ValidateEventRequest(
            dto.Title,
            dto.StartDate,
            dto.EndDate,
            dto.Status,
            dto.ExpectedAttendees,
            dto.MaxCapacity,
            dto.TicketPrice,
            dto.Currency,
            dto.Budget,
            dto.EventCategoryId,
            dto.VenueId);

        await EnsureCategoryExistsAsync(dto.EventCategoryId);
        await EnsureVenueExistsAsync(dto.VenueId);

        var evt = _mapper.Map<Event>(dto);
        NormalizeEvent(evt);
        evt.CreatedAt = DateTime.UtcNow;

        await _repo.AddAsync(evt);

        return (await GetByIdAsync(evt.Id))!;
    }

    public async Task<EventDto?> UpdateAsync(int id, UpdateEventDto dto)
    {
        ValidateEventRequest(
            dto.Title,
            dto.StartDate,
            dto.EndDate,
            dto.Status,
            dto.ExpectedAttendees,
            dto.MaxCapacity,
            dto.TicketPrice,
            dto.Currency,
            dto.Budget,
            dto.EventCategoryId,
            dto.VenueId);

        var evt = await _context.Events.FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        if (evt is null) return null;

        await EnsureCategoryExistsAsync(dto.EventCategoryId);
        await EnsureVenueExistsAsync(dto.VenueId);

        _mapper.Map(dto, evt);
        NormalizeEvent(evt);
        evt.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var evt = await _context.Events.FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        if (evt is null) return false;

        evt.IsActive = false;
        evt.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<EventDto?> UpdateStatusAsync(int id, string status)
    {
        var normalizedStatus = NormalizeEventStatus(status);

        var evt = await _context.Events.FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        if (evt is null) return null;

        evt.Status = normalizedStatus;
        evt.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    // Speaker assignments

    public async Task<EventSpeakerDto> AssignSpeakerAsync(int eventId, AssignSpeakerDto dto)
    {
        var evt = await _context.Events.FindAsync(eventId);
        if (evt is null || !evt.IsActive)
        {
            throw new KeyNotFoundException($"Event with ID {eventId} not found.");
        }

        var speaker = await _speakerRepo.GetByIdAsync(dto.SpeakerId);
        if (speaker is null || !speaker.IsActive)
        {
            throw new KeyNotFoundException($"Speaker with ID {dto.SpeakerId} not found.");
        }

        var existing = await _context.EventSpeakers
            .AnyAsync(es => es.EventId == eventId && es.SpeakerId == dto.SpeakerId);

        if (existing)
        {
            throw new InvalidOperationException("Speaker is already assigned to this event.");
        }

        var eventSpeaker = new EventSpeaker
        {
            EventId = eventId,
            SpeakerId = dto.SpeakerId,
            Role = TrimOptional(dto.Role),
            SortOrder = dto.SortOrder
        };

        _context.EventSpeakers.Add(eventSpeaker);
        await _context.SaveChangesAsync();

        await _context.Entry(eventSpeaker).Reference(es => es.Speaker).LoadAsync();
        return _mapper.Map<EventSpeakerDto>(eventSpeaker);
    }

    public async Task<bool> RemoveSpeakerAsync(int eventId, int speakerId)
    {
        var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId && e.IsActive);
        if (!eventExists) return false;

        var eventSpeaker = await _context.EventSpeakers
            .FirstOrDefaultAsync(x => x.EventId == eventId && x.SpeakerId == speakerId);

        if (eventSpeaker is null) return false;

        _context.EventSpeakers.Remove(eventSpeaker);
        await _context.SaveChangesAsync();
        return true;
    }

    // Timeline

    public async Task<EventTimelineDto> AddTimelineAsync(int eventId, CreateTimelineDto dto)
    {
        ValidateTimelineRequest(dto.Title, dto.DueDate, dto.Status);

        var evt = await _context.Events.FindAsync(eventId);
        if (evt is null || !evt.IsActive)
        {
            throw new KeyNotFoundException($"Event with ID {eventId} not found.");
        }

        var timeline = _mapper.Map<EventTimeline>(dto);
        NormalizeTimeline(timeline);
        timeline.EventId = eventId;
        timeline.CreatedAt = DateTime.UtcNow;
        ApplyTimelineCompletion(timeline);

        _context.EventTimelines.Add(timeline);
        await _context.SaveChangesAsync();

        return _mapper.Map<EventTimelineDto>(timeline);
    }

    public async Task<EventTimelineDto?> UpdateTimelineAsync(int eventId, int timelineId, UpdateTimelineDto dto)
    {
        ValidateTimelineRequest(dto.Title, dto.DueDate, dto.Status, dto.CompletedDate);

        var timeline = await _context.EventTimelines
            .FirstOrDefaultAsync(t => t.Id == timelineId && t.EventId == eventId && t.IsActive);

        if (timeline is null) return null;

        _mapper.Map(dto, timeline);
        NormalizeTimeline(timeline);
        timeline.UpdatedAt = DateTime.UtcNow;
        ApplyTimelineCompletion(timeline, dto.CompletedDate);

        await _context.SaveChangesAsync();
        return _mapper.Map<EventTimelineDto>(timeline);
    }

    public async Task<EventTimelineDto?> UpdateTimelineStatusAsync(int eventId, int timelineId, UpdateTimelineStatusDto dto)
    {
        var normalizedStatus = NormalizeTimelineStatus(dto.Status);

        var timeline = await _context.EventTimelines
            .FirstOrDefaultAsync(t => t.Id == timelineId && t.EventId == eventId && t.IsActive);

        if (timeline is null) return null;

        timeline.Status = normalizedStatus;
        timeline.UpdatedAt = DateTime.UtcNow;
        ApplyTimelineCompletion(timeline);

        await _context.SaveChangesAsync();
        return _mapper.Map<EventTimelineDto>(timeline);
    }

    public async Task<bool> RemoveTimelineAsync(int eventId, int timelineId)
    {
        var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId && e.IsActive);
        if (!eventExists) return false;

        var timeline = await _context.EventTimelines
            .FirstOrDefaultAsync(t => t.Id == timelineId && t.EventId == eventId && t.IsActive);

        if (timeline is null) return false;

        timeline.IsActive = false;
        timeline.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<Event> EventDetailsQuery()
    {
        return _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Include(e => e.EventSpeakers.OrderBy(es => es.SortOrder))
                .ThenInclude(es => es.Speaker)
            .Include(e => e.Timelines.Where(t => t.IsActive).OrderBy(t => t.SortOrder));
    }

    private async Task EnsureCategoryExistsAsync(int categoryId)
    {
        var category = await _categoryRepo.GetByIdAsync(categoryId);
        if (category is null || !category.IsActive)
        {
            throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
        }
    }

    private async Task EnsureVenueExistsAsync(int? venueId)
    {
        if (!venueId.HasValue) return;

        var venue = await _venueRepo.GetByIdAsync(venueId.Value);
        if (venue is null || !venue.IsActive)
        {
            throw new KeyNotFoundException($"Venue with ID {venueId.Value} not found.");
        }
    }

    private static void ValidateEventRequest(
        string? title,
        DateTime startDate,
        DateTime endDate,
        string? status,
        int? expectedAttendees,
        int? maxCapacity,
        decimal? ticketPrice,
        string? currency,
        decimal? budget,
        int eventCategoryId,
        int? venueId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.");
        }

        if (startDate == default)
        {
            throw new ArgumentException("Start date is required.");
        }

        if (endDate == default)
        {
            throw new ArgumentException("End date is required.");
        }

        if (endDate <= startDate)
        {
            throw new ArgumentException("End date must be later than start date.");
        }

        if (expectedAttendees is < 0)
        {
            throw new ArgumentException("Expected attendees cannot be negative.");
        }

        if (maxCapacity is <= 0)
        {
            throw new ArgumentException("Max capacity must be greater than zero when provided.");
        }

        if (expectedAttendees.HasValue && maxCapacity.HasValue && expectedAttendees > maxCapacity)
        {
            throw new ArgumentException("Expected attendees cannot exceed max capacity.");
        }

        if (ticketPrice is < 0)
        {
            throw new ArgumentException("Ticket price cannot be negative.");
        }

        if (budget is < 0)
        {
            throw new ArgumentException("Budget cannot be negative.");
        }

        if (eventCategoryId <= 0)
        {
            throw new ArgumentException("Event category is required.");
        }

        if (venueId is <= 0)
        {
            throw new ArgumentException("Venue ID must be greater than zero when provided.");
        }

        _ = NormalizeEventStatus(status);
        _ = NormalizeCurrency(currency);
    }

    private static void ValidateTimelineRequest(
        string? title,
        DateTime dueDate,
        string? status,
        DateTime? completedDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.");
        }

        if (dueDate == default)
        {
            throw new ArgumentException("Due date is required.");
        }

        var normalizedStatus = NormalizeTimelineStatus(status);
        if (completedDate.HasValue && normalizedStatus != TimelineStatus.Completed)
        {
            throw new ArgumentException("Completed date can only be set when status is Completed.");
        }
    }

    private static void NormalizeEvent(Event evt)
    {
        evt.Title = evt.Title.Trim();
        evt.Description = TrimOptional(evt.Description);
        evt.ShortDescription = TrimOptional(evt.ShortDescription);
        evt.ImageUrl = TrimOptional(evt.ImageUrl);
        evt.Status = NormalizeEventStatus(evt.Status);
        evt.Currency = NormalizeCurrency(evt.Currency);
        evt.Objectives = TrimOptional(evt.Objectives);
        evt.RiskManagementPlan = TrimOptional(evt.RiskManagementPlan);
        evt.PermitsNotes = TrimOptional(evt.PermitsNotes);
        evt.TransportationNotes = TrimOptional(evt.TransportationNotes);
        evt.EquipmentNotes = TrimOptional(evt.EquipmentNotes);
        evt.CateringNotes = TrimOptional(evt.CateringNotes);
    }

    private static void NormalizeTimeline(EventTimeline timeline)
    {
        timeline.Title = timeline.Title.Trim();
        timeline.Description = TrimOptional(timeline.Description);
        timeline.Status = NormalizeTimelineStatus(timeline.Status);
        timeline.AssignedTo = TrimOptional(timeline.AssignedTo);
    }

    private static void ApplyTimelineCompletion(EventTimeline timeline, DateTime? requestedCompletedDate = null)
    {
        timeline.CompletedDate = timeline.Status == TimelineStatus.Completed
            ? requestedCompletedDate ?? timeline.CompletedDate ?? DateTime.UtcNow
            : null;
    }

    private static string NormalizeEventStatus(string? status)
        => NormalizeStatus(status, EventStatus.All, "event status");

    private static string NormalizeTimelineStatus(string? status)
        => NormalizeStatus(status, TimelineStatus.All, "timeline status");

    private static string NormalizeStatus(string? status, IReadOnlyCollection<string> allowedStatuses, string label)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException($"{label} is required.");
        }

        var normalizedStatus = allowedStatuses.FirstOrDefault(allowedStatus =>
            string.Equals(allowedStatus, status.Trim(), StringComparison.OrdinalIgnoreCase));

        if (normalizedStatus is null)
        {
            throw new ArgumentException($"Invalid {label} '{status}'. Valid values: {string.Join(", ", allowedStatuses)}.");
        }

        return normalizedStatus;
    }

    private static string NormalizeCurrency(string? currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.");
        }

        var normalizedCurrency = currency.Trim().ToUpperInvariant();
        if (normalizedCurrency.Length != 3)
        {
            throw new ArgumentException("Currency must be a 3-letter code.");
        }

        return normalizedCurrency;
    }

    private static string? TrimOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
