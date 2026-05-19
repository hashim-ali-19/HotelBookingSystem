using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.Services;

public class RoomSearchFilter : IValidatableObject
{
    public string? City { get; set; }

    public string? Keyword { get; set; }

    public DateTime CheckIn { get; set; } = DateTime.Today.AddDays(1);

    public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(2);

    [Range(1, 12)]
    public int Guests { get; set; } = 2;

    public RoomType? Type { get; set; }

    public decimal? MaxPrice { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckIn.Date < DateTime.Today)
        {
            yield return new ValidationResult("Check-in date cannot be in the past.", new[] { nameof(CheckIn) });
        }

        if (CheckOut.Date <= CheckIn.Date)
        {
            yield return new ValidationResult("Check-out date must be after check-in.", new[] { nameof(CheckOut) });
        }
    }
}
