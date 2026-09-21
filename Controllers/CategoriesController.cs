using Microsoft.AspNetCore.Mvc;
using EP.API.DTOs;
using EP.API.Services;

namespace EP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll()
        => Ok(await _categoryService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var cat = await _categoryService.GetByIdAsync(id);
        if (cat is null) return NotFound();
        return Ok(cat);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var cat = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = cat.Id }, cat);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        var cat = await _categoryService.UpdateAsync(id, dto);
        if (cat is null) return NotFound();
        return Ok(cat);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
