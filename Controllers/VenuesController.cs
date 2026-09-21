using Microsoft.AspNetCore.Mvc;
using EP.API.DTOs;
using EP.API.Services;

namespace EP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenuesController(IVenueService venueService) => _venueService = venueService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VenueDto>>> GetAll()
        => Ok(await _venueService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VenueDto>> GetById(int id)
    {
        var venue = await _venueService.GetByIdAsync(id);
        if (venue is null) return NotFound();
        return Ok(venue);
    }

    [HttpPost]
    public async Task<ActionResult<VenueDto>> Create([FromBody] CreateVenueDto dto)
    {
        var venue = await _venueService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = venue.Id }, venue);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<VenueDto>> Update(int id, [FromBody] UpdateVenueDto dto)
    {
        var venue = await _venueService.UpdateAsync(id, dto);
        if (venue is null) return NotFound();
        return Ok(venue);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _venueService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
