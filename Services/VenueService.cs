using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EP.API.Data;
using EP.API.DTOs;
using EP.API.Entities;
using EP.API.Repositories;

namespace EP.API.Services;

public class VenueService : IVenueService
{
    private readonly AppDbContext _context;
    private readonly IRepository<Venue> _repo;
    private readonly IMapper _mapper;

    public VenueService(AppDbContext context, IRepository<Venue> repo, IMapper mapper)
    {
        _context = context;
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<VenueDto>> GetAllAsync()
    {
        var venues = await _context.Venues
            .AsNoTracking()
            .Include(v => v.Events.Where(e => e.IsActive))
            .Where(v => v.IsActive)
            .OrderBy(v => v.Name)
            .ToListAsync();

        return _mapper.Map<List<VenueDto>>(venues);
    }

    public async Task<VenueDto?> GetByIdAsync(int id)
    {
        var venue = await _context.Venues
            .AsNoTracking()
            .Include(v => v.Events.Where(e => e.IsActive))
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        return venue is null ? null : _mapper.Map<VenueDto>(venue);
    }

    public async Task<VenueDto> CreateAsync(CreateVenueDto dto)
    {
        ValidateVenue(dto.Name, dto.Address);

        var venue = _mapper.Map<Venue>(dto);
        NormalizeVenue(venue);
        await EnsureUniqueVenueAsync(venue.Name, venue.Address);

        venue.CreatedAt = DateTime.UtcNow;
        await _repo.AddAsync(venue);
        return _mapper.Map<VenueDto>(venue);
    }

    public async Task<VenueDto?> UpdateAsync(int id, UpdateVenueDto dto)
    {
        ValidateVenue(dto.Name, dto.Address);

        var venue = await _context.Venues
            .Include(v => v.Events)
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        if (venue is null) return null;

        _mapper.Map(dto, venue);
        NormalizeVenue(venue);
        await EnsureUniqueVenueAsync(venue.Name, venue.Address, id);

        if (!venue.IsActive && venue.Events.Any(e => e.IsActive))
        {
            throw new InvalidOperationException("Venue cannot be deactivated while active events reference it.");
        }

        venue.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync();

        return _mapper.Map<VenueDto>(venue);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var venue = await _context.Venues
            .Include(v => v.Events)
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        if (venue is null) return false;

        if (venue.Events.Any(e => e.IsActive))
        {
            throw new InvalidOperationException("Venue cannot be deleted while active events reference it.");
        }

        venue.IsActive = false;
        venue.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync();
        return true;
    }

    private async Task EnsureUniqueVenueAsync(string name, string address, int? excludedId = null)
    {
        var exists = await _context.Venues.AnyAsync(v =>
            v.IsActive &&
            v.Name == name &&
            v.Address == address &&
            (!excludedId.HasValue || v.Id != excludedId.Value));

        if (exists)
        {
            throw new InvalidOperationException($"Venue '{name}' at '{address}' already exists.");
        }
    }

    private static void ValidateVenue(string? name, string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address is required.");
        }
    }

    private static void NormalizeVenue(Venue venue)
    {
        venue.Name = venue.Name.Trim();
        venue.Description = TrimOptional(venue.Description);
        venue.Address = venue.Address.Trim();
        venue.City = TrimOptional(venue.City);
        venue.State = TrimOptional(venue.State);
        venue.ZipCode = TrimOptional(venue.ZipCode);
        venue.Country = TrimOptional(venue.Country);
        venue.ContactPhone = TrimOptional(venue.ContactPhone);
        venue.ContactEmail = TrimOptional(venue.ContactEmail);
        venue.Amenities = TrimOptional(venue.Amenities);
    }

    private static string? TrimOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
