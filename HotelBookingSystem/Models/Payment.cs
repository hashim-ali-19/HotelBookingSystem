using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models;

public class Payment : BaseEntity
{
    [Required]
    public int BookingId { get; set; }

    public Booking? Booking { get; set; }

    [Required, StringLength(80)]
    public string PayerName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; } = PaymentMethod.Card;

    public PaymentStatus Status { get; set; } = PaymentStatus.Paid;

    [Required, StringLength(40)]
    public string TransactionReference { get; set; } = string.Empty;

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    public override string GetDisplayName()
    {
        return $"{TransactionReference} - PKR {Amount:N0}";
    }
}
