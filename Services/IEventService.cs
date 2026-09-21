using EP.API.DTOs;

namespace EP.API.Services;

public interface IEventService
{
    Task<IReadOnlyList<EventDto>> GetAllAsync();
    Task<EventDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<EventDto>> GetByCategoryAsync(int categoryId);
    Task<IReadOnlyList<EventDto>> GetByStatusAsync(string status);
    Task<IReadOnlyList<EventDto>> GetUpcomingAsync(int count = 10);
    Task<EventDto> CreateAsync(CreateEventDto dto);
    Task<EventDto?> UpdateAsync(int id, UpdateEventDto dto);
    Task<bool> DeleteAsync(int id);
    Task<EventDto?> UpdateStatusAsync(int id, string status);

    // Speaker assignments
    Task<EventSpeakerDto> AssignSpeakerAsync(int eventId, AssignSpeakerDto dto);
    Task<bool> RemoveSpeakerAsync(int eventId, int speakerId);

    // Timeline
    Task<EventTimelineDto> AddTimelineAsync(int eventId, CreateTimelineDto dto);
    Task<EventTimelineDto?> UpdateTimelineAsync(int eventId, int timelineId, UpdateTimelineDto dto);
    Task<EventTimelineDto?> UpdateTimelineStatusAsync(int eventId, int timelineId, UpdateTimelineStatusDto dto);
    Task<bool> RemoveTimelineAsync(int eventId, int timelineId);
}
