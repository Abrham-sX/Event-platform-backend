using EP.API.DTOs;

namespace EP.API.Services;

public interface IVenueService
{
    Task<IReadOnlyList<VenueDto>> GetAllAsync();
    Task<VenueDto?> GetByIdAsync(int id);
    Task<VenueDto> CreateAsync(CreateVenueDto dto);
    Task<VenueDto?> UpdateAsync(int id, UpdateVenueDto dto);
    Task<bool> DeleteAsync(int id);
}
