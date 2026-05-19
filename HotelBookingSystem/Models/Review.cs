using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models;

public class Review : BaseEntity
{
    [Required]
    public int HotelId { get; set; }

    public Hotel? Hotel { get; set; }

    public int? UserId { get; set; }

    public User? User { get; set; }

    [Required, StringLength(80)]
    public string ReviewerName { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [Required, StringLength(600)]
    public string Comment { get; set; } = string.Empty;

    public bool IsApproved { get; set; } = true;

    public override string GetDisplayName()
    {
        return $"{ReviewerName} rated {Rating}/5";
    }
}
