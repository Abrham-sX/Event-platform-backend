using Microsoft.AspNetCore.Mvc;
using EP.API.DTOs;
using EP.API.Services;

namespace EP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService) => _eventService = eventService;

    /// <summary>Get all active events.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetAll()
        => Ok(await _eventService.GetAllAsync());

    /// <summary>Get an event by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventDto>> GetById(int id)
    {
        var evt = await _eventService.GetByIdAsync(id);
        if (evt is null) return NotFound();
        return Ok(evt);
    }

    /// <summary>Get events by category.</summary>
    [HttpGet("by-category/{categoryId:int}")]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetByCategory(int categoryId)
        => Ok(await _eventService.GetByCategoryAsync(categoryId));

    /// <summary>Get events by status (Draft, Published, etc.).</summary>
    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetByStatus(string status)
        => Ok(await _eventService.GetByStatusAsync(status));

    /// <summary>Get upcoming published events.</summary>
    [HttpGet("upcoming")]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetUpcoming([FromQuery] int count = 10)
        => Ok(await _eventService.GetUpcomingAsync(count));

    /// <summary>Create a new event.</summary>
    [HttpPost]
    public async Task<ActionResult<EventDto>> Create([FromBody] CreateEventDto dto)
    {
        var evt = await _eventService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = evt.Id }, evt);
    }

    /// <summary>Update an existing event.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<EventDto>> Update(int id, [FromBody] UpdateEventDto dto)
    {
        var evt = await _eventService.UpdateAsync(id, dto);
        if (evt is null) return NotFound();
        return Ok(evt);
    }

    /// <summary>Update event status.</summary>
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<EventDto>> UpdateStatus(int id, [FromBody] UpdateEventStatusDto request)
    {
        var evt = await _eventService.UpdateStatusAsync(id, request.Status);
        if (evt is null) return NotFound();
        return Ok(evt);
    }

    /// <summary>Soft-delete an event.</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _eventService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // ── Speaker assignments ────────────────────────────────────

    /// <summary>Assign a speaker to an event.</summary>
    [HttpPost("{eventId:int}/speakers")]
    public async Task<ActionResult<EventSpeakerDto>> AssignSpeaker(int eventId, [FromBody] AssignSpeakerDto dto)
        => Ok(await _eventService.AssignSpeakerAsync(eventId, dto));

    /// <summary>Remove a speaker from an event.</summary>
    [HttpDelete("{eventId:int}/speakers/{speakerId:int}")]
    public async Task<ActionResult> RemoveSpeaker(int eventId, int speakerId)
    {
        var removed = await _eventService.RemoveSpeakerAsync(eventId, speakerId);
        if (!removed) return NotFound();
        return NoContent();
    }

    // ── Timeline ───────────────────────────────────────────────

    /// <summary>Add a timeline item to an event.</summary>
    [HttpPost("{eventId:int}/timelines")]
    public async Task<ActionResult<EventTimelineDto>> AddTimeline(int eventId, [FromBody] CreateTimelineDto dto)
        => Ok(await _eventService.AddTimelineAsync(eventId, dto));

    /// <summary>Update a timeline item.</summary>
    [HttpPut("{eventId:int}/timelines/{timelineId:int}")]
    public async Task<ActionResult<EventTimelineDto>> UpdateTimeline(int eventId, int timelineId, [FromBody] UpdateTimelineDto dto)
    {
        var tl = await _eventService.UpdateTimelineAsync(eventId, timelineId, dto);
        if (tl is null) return NotFound();
        return Ok(tl);
    }

    /// <summary>Update timeline status (Pending, InProgress, Completed, Overdue).</summary>
    [HttpPatch("{eventId:int}/timelines/{timelineId:int}/status")]
    public async Task<ActionResult<EventTimelineDto>> UpdateTimelineStatus(int eventId, int timelineId, [FromBody] UpdateTimelineStatusDto dto)
    {
        var tl = await _eventService.UpdateTimelineStatusAsync(eventId, timelineId, dto);
        if (tl is null) return NotFound();
        return Ok(tl);
    }

    /// <summary>Remove a timeline item.</summary>
    [HttpDelete("{eventId:int}/timelines/{timelineId:int}")]
    public async Task<ActionResult> RemoveTimeline(int eventId, int timelineId)
    {
        var removed = await _eventService.RemoveTimelineAsync(eventId, timelineId);
        if (!removed) return NotFound();
        return NoContent();
    }
}

