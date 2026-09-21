using Microsoft.AspNetCore.Mvc;
using EP.API.DTOs;
using EP.API.Services;

namespace EP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SpeakersController : ControllerBase
{
    private readonly ISpeakerService _speakerService;

    public SpeakersController(ISpeakerService speakerService) => _speakerService = speakerService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SpeakerDto>>> GetAll()
        => Ok(await _speakerService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpeakerDto>> GetById(int id)
    {
        var speaker = await _speakerService.GetByIdAsync(id);
        if (speaker is null) return NotFound();
        return Ok(speaker);
    }

    [HttpPost]
    public async Task<ActionResult<SpeakerDto>> Create([FromBody] CreateSpeakerDto dto)
    {
        var speaker = await _speakerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = speaker.Id }, speaker);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SpeakerDto>> Update(int id, [FromBody] UpdateSpeakerDto dto)
    {
        var speaker = await _speakerService.UpdateAsync(id, dto);
        if (speaker is null) return NotFound();
        return Ok(speaker);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _speakerService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
