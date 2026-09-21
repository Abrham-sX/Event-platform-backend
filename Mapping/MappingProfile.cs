using AutoMapper;
using EP.API.DTOs;
using EP.API.Entities;

namespace EP.API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── Event ──────────────────────────────────────────────
        CreateMap<Event, EventDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.VenueId, o => o.MapFrom(s => s.Venue != null && s.Venue.IsActive ? s.VenueId : null))
            .ForMember(d => d.VenueName, o => o.MapFrom(s => s.Venue != null && s.Venue.IsActive ? s.Venue.Name : null))
            .ForMember(d => d.VenueAddress, o => o.MapFrom(s => s.Venue != null && s.Venue.IsActive ? s.Venue.Address : null))
            .ForMember(d => d.VenueCity, o => o.MapFrom(s => s.Venue != null && s.Venue.IsActive ? s.Venue.City : null))
            .ForMember(d => d.Speakers, o => o.MapFrom(s => s.EventSpeakers
                .Where(es => es.Speaker != null && es.Speaker.IsActive)
                .OrderBy(es => es.SortOrder)))
            .ForMember(d => d.Timelines, o => o.MapFrom(s => s.Timelines
                .Where(t => t.IsActive)
                .OrderBy(t => t.SortOrder)));

        CreateMap<CreateEventDto, Event>();
        CreateMap<UpdateEventDto, Event>();

        // ── Category ───────────────────────────────────────────
        CreateMap<EventCategory, CategoryDto>()
            .ForMember(d => d.EventCount, o => o.MapFrom(s => s.Events.Count(e => e.IsActive)));

        CreateMap<CreateCategoryDto, EventCategory>();
        CreateMap<UpdateCategoryDto, EventCategory>();

        // ── Venue ──────────────────────────────────────────────
        CreateMap<Venue, VenueDto>()
            .ForMember(d => d.EventCount, o => o.MapFrom(s => s.Events.Count(e => e.IsActive)));

        CreateMap<CreateVenueDto, Venue>();
        CreateMap<UpdateVenueDto, Venue>();

        // ── Speaker ────────────────────────────────────────────
        CreateMap<Speaker, SpeakerDto>();
        CreateMap<CreateSpeakerDto, Speaker>();
        CreateMap<UpdateSpeakerDto, Speaker>();

        // ── EventSpeaker ───────────────────────────────────────
        CreateMap<EventSpeaker, EventSpeakerDto>()
            .ForMember(d => d.SpeakerId, o => o.MapFrom(s => s.SpeakerId))
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.Speaker != null ? s.Speaker.FirstName : null))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.Speaker != null ? s.Speaker.LastName : null))
            .ForMember(d => d.PhotoUrl, o => o.MapFrom(s => s.Speaker != null ? s.Speaker.PhotoUrl : null))
            .ForMember(d => d.Company, o => o.MapFrom(s => s.Speaker != null ? s.Speaker.Company : null))
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Speaker != null ? s.Speaker.Title : null));

        // ── Timeline ───────────────────────────────────────────
        CreateMap<EventTimeline, EventTimelineDto>();
        CreateMap<CreateTimelineDto, EventTimeline>();
        CreateMap<UpdateTimelineDto, EventTimeline>();
    }
}
