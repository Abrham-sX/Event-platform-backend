using EP.API.DTOs;

namespace EP.API.Services;

public interface ISpeakerService
{
    Task<IReadOnlyList<SpeakerDto>> GetAllAsync();
    Task<SpeakerDto?> GetByIdAsync(int id);
    Task<SpeakerDto> CreateAsync(CreateSpeakerDto dto);
    Task<SpeakerDto?> UpdateAsync(int id, UpdateSpeakerDto dto);
    Task<bool> DeleteAsync(int id);
}
