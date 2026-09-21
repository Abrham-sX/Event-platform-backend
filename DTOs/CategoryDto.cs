using System.ComponentModel.DataAnnotations;

namespace EP.API.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; }
    public int EventCount { get; set; }
}

public class CreateCategoryDto : IValidatableObject
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? IconUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var nameResult = DtoValidation.RequiredText(Name, nameof(Name), "Name");
        if (nameResult is not null) yield return nameResult;
    }
}

public class UpdateCategoryDto : IValidatableObject
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? IconUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var nameResult = DtoValidation.RequiredText(Name, nameof(Name), "Name");
        if (nameResult is not null) yield return nameResult;
    }
}
