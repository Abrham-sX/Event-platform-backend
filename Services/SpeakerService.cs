using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EP.API.Data;
using EP.API.DTOs;
using EP.API.Entities;
using EP.API.Repositories;

namespace EP.API.Services;

public class SpeakerService : ISpeakerService
{
    private readonly AppDbContext _context;
    private readonly IRepository<Speaker> _repo;
    private readonly IMapper _mapper;

    public SpeakerService(AppDbContext context, IRepository<Speaker> repo, IMapper mapper)
    {
        _context = context;
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SpeakerDto>> GetAllAsync()
    {
        var speakers = await _context.Speakers
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();

        return _mapper.Map<List<SpeakerDto>>(speakers);
    }

    public async Task<SpeakerDto?> GetByIdAsync(int id)
    {
        var speaker = await _context.Speakers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

        return speaker is null ? null : _mapper.Map<SpeakerDto>(speaker);
    }

    public async Task<SpeakerDto> CreateAsync(CreateSpeakerDto dto)
    {
        ValidateSpeaker(dto.FirstName, dto.LastName);

        var speaker = _mapper.Map<Speaker>(dto);
        NormalizeSpeaker(speaker);
        await EnsureUniqueEmailAsync(speaker.Email);

        speaker.CreatedAt = DateTime.UtcNow;
        await _repo.AddAsync(speaker);
        return _mapper.Map<SpeakerDto>(speaker);
    }

    public async Task<SpeakerDto?> UpdateAsync(int id, UpdateSpeakerDto dto)
    {
        ValidateSpeaker(dto.FirstName, dto.LastName);

        var speaker = await _context.Speakers
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

        if (speaker is null) return null;

        _mapper.Map(dto, speaker);
        NormalizeSpeaker(speaker);
        await EnsureUniqueEmailAsync(speaker.Email, id);

        if (!speaker.IsActive && await HasActiveEventAssignmentsAsync(id))
        {
            throw new InvalidOperationException("Speaker cannot be deactivated while assigned to active events.");
        }

        speaker.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync();

        return _mapper.Map<SpeakerDto>(speaker);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var speaker = await _context.Speakers
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

        if (speaker is null) return false;

        if (await HasActiveEventAssignmentsAsync(id))
        {
            throw new InvalidOperationException("Speaker cannot be deleted while assigned to active events.");
        }

        speaker.IsActive = false;
        speaker.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync();
        return true;
    }

    private async Task EnsureUniqueEmailAsync(string? email, int? excludedId = null)
    {
        if (email is null) return;

        var exists = await _context.Speakers.AnyAsync(s =>
            s.IsActive &&
            s.Email == email &&
            (!excludedId.HasValue || s.Id != excludedId.Value));

        if (exists)
        {
            throw new InvalidOperationException($"Speaker with email '{email}' already exists.");
        }
    }

    private async Task<bool> HasActiveEventAssignmentsAsync(int speakerId)
    {
        return await _context.EventSpeakers
            .Include(es => es.Event)
            .AnyAsync(es => es.SpeakerId == speakerId && es.Event.IsActive);
    }

    private static void ValidateSpeaker(string? firstName, string? lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.");
        }
    }

    private static void NormalizeSpeaker(Speaker speaker)
    {
        speaker.FirstName = speaker.FirstName.Trim();
        speaker.LastName = speaker.LastName.Trim();
        speaker.Bio = TrimOptional(speaker.Bio);
        speaker.PhotoUrl = TrimOptional(speaker.PhotoUrl);
        speaker.Email = TrimOptional(speaker.Email)?.ToLowerInvariant();
        speaker.Phone = TrimOptional(speaker.Phone);
        speaker.Company = TrimOptional(speaker.Company);
        speaker.Title = TrimOptional(speaker.Title);
        speaker.Website = TrimOptional(speaker.Website);
    }

    private static string? TrimOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
