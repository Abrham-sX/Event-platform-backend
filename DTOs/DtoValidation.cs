using System.ComponentModel.DataAnnotations;
using EP.API.Entities;

namespace EP.API.DTOs;

internal static class DtoValidation
{
    public static ValidationResult? RequiredText(string? value, string memberName, string displayName)
    {
        return string.IsNullOrWhiteSpace(value)
            ? new ValidationResult($"{displayName} is required.", new[] { memberName })
            : null;
    }

    public static ValidationResult? OneOf(string? value, IReadOnlyCollection<string> allowedValues, string memberName, string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationResult($"{displayName} is required.", new[] { memberName });
        }

        var isValid = allowedValues.Any(allowedValue =>
            string.Equals(allowedValue, value.Trim(), StringComparison.OrdinalIgnoreCase));

        return isValid
            ? null
            : new ValidationResult($"{displayName} must be one of: {string.Join(", ", allowedValues)}.", new[] { memberName });
    }

    public static IEnumerable<ValidationResult> ValidateEvent(
        string? title,
        DateTime startDate,
        DateTime endDate,
        string? status,
        int? expectedAttendees,
        int? maxCapacity,
        decimal? ticketPrice,
        string? currency,
        decimal? budget,
        int eventCategoryId,
        int? venueId)
    {
        var titleResult = RequiredText(title, "Title", "Title");
        if (titleResult is not null) yield return titleResult;

        var statusResult = OneOf(status, EventStatus.All, "Status", "Status");
        if (statusResult is not null) yield return statusResult;

        if (startDate == default)
        {
            yield return new ValidationResult("Start date is required.", new[] { "StartDate" });
        }

        if (endDate == default)
        {
            yield return new ValidationResult("End date is required.", new[] { "EndDate" });
        }

        if (startDate != default && endDate != default && endDate <= startDate)
        {
            yield return new ValidationResult(
                "End date must be later than start date.",
                new[] { "StartDate", "EndDate" });
        }

        if (expectedAttendees is < 0)
        {
            yield return new ValidationResult(
                "Expected attendees cannot be negative.",
                new[] { "ExpectedAttendees" });
        }

        if (maxCapacity is <= 0)
        {
            yield return new ValidationResult(
                "Max capacity must be greater than zero when provided.",
                new[] { "MaxCapacity" });
        }

        if (expectedAttendees.HasValue && maxCapacity.HasValue && expectedAttendees > maxCapacity)
        {
            yield return new ValidationResult(
                "Expected attendees cannot exceed max capacity.",
                new[] { "ExpectedAttendees", "MaxCapacity" });
        }

        if (ticketPrice is < 0)
        {
            yield return new ValidationResult("Ticket price cannot be negative.", new[] { "TicketPrice" });
        }

        if (budget is < 0)
        {
            yield return new ValidationResult("Budget cannot be negative.", new[] { "Budget" });
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            yield return new ValidationResult("Currency is required.", new[] { "Currency" });
        }
        else if (currency.Trim().Length != 3)
        {
            yield return new ValidationResult("Currency must be a 3-letter code.", new[] { "Currency" });
        }

        if (eventCategoryId <= 0)
        {
            yield return new ValidationResult("Event category is required.", new[] { "EventCategoryId" });
        }

        if (venueId is <= 0)
        {
            yield return new ValidationResult("Venue ID must be greater than zero when provided.", new[] { "VenueId" });
        }
    }

    public static IEnumerable<ValidationResult> ValidateTimeline(
        string? title,
        DateTime dueDate,
        string? status,
        DateTime? completedDate = null)
    {
        var titleResult = RequiredText(title, "Title", "Title");
        if (titleResult is not null) yield return titleResult;

        var statusResult = OneOf(status, TimelineStatus.All, "Status", "Status");
        if (statusResult is not null) yield return statusResult;

        if (dueDate == default)
        {
            yield return new ValidationResult("Due date is required.", new[] { "DueDate" });
        }

        var isCompleted = string.Equals(status?.Trim(), TimelineStatus.Completed, StringComparison.OrdinalIgnoreCase);
        if (completedDate.HasValue && !isCompleted)
        {
            yield return new ValidationResult(
                "Completed date can only be set when status is Completed.",
                new[] { "CompletedDate", "Status" });
        }
    }
}
