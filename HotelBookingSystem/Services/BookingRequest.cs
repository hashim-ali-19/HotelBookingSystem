using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Services;

public class BookingRequest : IValidatableObject
{
    [Required]
    public int RoomId { get; set; }

    [Range(1, 1, ErrorMessage = "Only one room unit can be booked at a time.")]
    public int RoomUnits { get; set; } = 1;

    [Required, StringLength(80)]
    public string GuestName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string GuestEmail { get; set; } = string.Empty;

    [Required, Phone, StringLength(25)]
    public string GuestPhone { get; set; } = string.Empty;

    public DateTime CheckIn { get; set; } = DateTime.Today.AddDays(1);

    public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(2);

    [Range(1, 12)]
    public int Guests { get; set; } = 2;

    [StringLength(500)]
    public string SpecialRequest { get; set; } = string.Empty;

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
