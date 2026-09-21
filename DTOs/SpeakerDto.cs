using System.ComponentModel.DataAnnotations;

namespace EP.API.DTOs;

public class SpeakerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? Title { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSpeakerDto : IValidatableObject
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Bio { get; set; }

    [StringLength(1000)]
    public string? PhotoUrl { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Company { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [Url]
    [StringLength(500)]
    public string? Website { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var firstNameResult = DtoValidation.RequiredText(FirstName, nameof(FirstName), "First name");
        if (firstNameResult is not null) yield return firstNameResult;

        var lastNameResult = DtoValidation.RequiredText(LastName, nameof(LastName), "Last name");
        if (lastNameResult is not null) yield return lastNameResult;
    }
}

public class UpdateSpeakerDto : IValidatableObject
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Bio { get; set; }

    [StringLength(1000)]
    public string? PhotoUrl { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Company { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [Url]
    [StringLength(500)]
    public string? Website { get; set; }

    public bool IsActive { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var firstNameResult = DtoValidation.RequiredText(FirstName, nameof(FirstName), "First name");
        if (firstNameResult is not null) yield return firstNameResult;

        var lastNameResult = DtoValidation.RequiredText(LastName, nameof(LastName), "Last name");
        if (lastNameResult is not null) yield return lastNameResult;
    }
}

public class EventSpeakerDto
{
    public int Id { get; set; }
    public int SpeakerId { get; set; }
    public string? Role { get; set; }
    public int SortOrder { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Company { get; set; }
    public string? Title { get; set; }
}

public class AssignSpeakerDto
{
    [Range(1, int.MaxValue)]
    public int SpeakerId { get; set; }

    [StringLength(100)]
    public string? Role { get; set; }

    public int SortOrder { get; set; } = 0;
}
