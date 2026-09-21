using System.ComponentModel.DataAnnotations;

namespace EP.API.DTOs;

public class VenueDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public int? Capacity { get; set; }
    public string? Amenities { get; set; }
    public bool IsActive { get; set; }
    public int EventCount { get; set; }
}

public class CreateVenueDto : IValidatableObject
{
    [Required]
    [StringLength(250)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [StringLength(150)]
    public string? City { get; set; }

    [StringLength(150)]
    public string? State { get; set; }

    [StringLength(20)]
    public string? ZipCode { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [Range(-90.0, 90.0)]
    public double? Latitude { get; set; }

    [Range(-180.0, 180.0)]
    public double? Longitude { get; set; }

    [Phone]
    [StringLength(50)]
    public string? ContactPhone { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? ContactEmail { get; set; }

    [Range(1, int.MaxValue)]
    public int? Capacity { get; set; }

    [StringLength(2000)]
    public string? Amenities { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var nameResult = DtoValidation.RequiredText(Name, nameof(Name), "Name");
        if (nameResult is not null) yield return nameResult;

        var addressResult = DtoValidation.RequiredText(Address, nameof(Address), "Address");
        if (addressResult is not null) yield return addressResult;
    }
}

public class UpdateVenueDto : IValidatableObject
{
    [Required]
    [StringLength(250)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [StringLength(150)]
    public string? City { get; set; }

    [StringLength(150)]
    public string? State { get; set; }

    [StringLength(20)]
    public string? ZipCode { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [Range(-90.0, 90.0)]
    public double? Latitude { get; set; }

    [Range(-180.0, 180.0)]
    public double? Longitude { get; set; }

    [Phone]
    [StringLength(50)]
    public string? ContactPhone { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? ContactEmail { get; set; }

    [Range(1, int.MaxValue)]
    public int? Capacity { get; set; }

    [StringLength(2000)]
    public string? Amenities { get; set; }

    public bool IsActive { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var nameResult = DtoValidation.RequiredText(Name, nameof(Name), "Name");
        if (nameResult is not null) yield return nameResult;

        var addressResult = DtoValidation.RequiredText(Address, nameof(Address), "Address");
        if (addressResult is not null) yield return addressResult;
    }
}
