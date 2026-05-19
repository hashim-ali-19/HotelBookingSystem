using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models;

public class Booking : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    public User? User { get; set; }

    [Required]
    public int RoomId { get; set; }

    public Room? Room { get; set; }

    [Required, StringLength(20)]
    public string BookingCode { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string GuestName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string GuestEmail { get; set; } = string.Empty;

    [Required, Phone, StringLength(25)]
    public string GuestPhone { get; set; } = string.Empty;

    public DateTime CheckIn { get; set; } = DateTime.Today.AddDays(1);

    public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(2);

    [Range(1, 12)]
    public int Guests { get; set; } = 1;

    [StringLength(500)]
    public string SpecialRequest { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [NotMapped]
    public int Nights => Math.Max(1, (CheckOut.Date - CheckIn.Date).Days);

    public override string GetDisplayName()
    {
        return $"{BookingCode} - {GuestName}";
    }
}
