using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EP.API.Data;
using EP.API.DTOs;
using EP.API.Entities;
using EP.API.Repositories;

namespace EP.API.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly IRepository<EventCategory> _repo;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext context, IRepository<EventCategory> repo, IMapper mapper)
    {
        _context = context;
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync()
    {
        var categories = await _context.EventCategories
            .AsNoTracking()
            .Include(c => c.Events.Where(e => e.IsActive))
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return _mapper.Map<List<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _context.EventCategories
            .AsNoTracking()
            .Include(c => c.Events.Where(e => e.IsActive))
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        return category is null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        ValidateCategory(dto.Name);

        var category = _mapper.Map<EventCategory>(dto);
        NormalizeCategory(category);
        await EnsureUniqueNameAsync(category.Name);

        category.CreatedAt = DateTime.UtcNow;
        await _repo.AddAsync(category);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        ValidateCategory(dto.Name);

        var category = await _context.EventCategories
            .Include(c => c.Events)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (category is null) return null;

        _mapper.Map(dto, category);
        NormalizeCategory(category);
        await EnsureUniqueNameAsync(category.Name, id);

        if (!category.IsActive && category.Events.Any(e => e.IsActive))
        {
            throw new InvalidOperationException("Category cannot be deactivated while active events reference it.");
        }

        category.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.EventCategories
            .Include(c => c.Events)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (category is null) return false;

        if (category.Events.Any(e => e.IsActive))
        {
            throw new InvalidOperationException("Category cannot be deleted while active events reference it.");
        }

        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync();
        return true;
    }

    private async Task EnsureUniqueNameAsync(string name, int? excludedId = null)
    {
        var exists = await _context.EventCategories.AnyAsync(c =>
            c.IsActive &&
            c.Name == name &&
            (!excludedId.HasValue || c.Id != excludedId.Value));

        if (exists)
        {
            throw new InvalidOperationException($"Category '{name}' already exists.");
        }
    }

    private static void ValidateCategory(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.");
        }
    }

    private static void NormalizeCategory(EventCategory category)
    {
        category.Name = category.Name.Trim();
        category.Description = TrimOptional(category.Description);
        category.IconUrl = TrimOptional(category.IconUrl);
    }

    private static string? TrimOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
