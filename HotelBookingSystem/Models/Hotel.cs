using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models;

public class Hotel : BaseEntity
{
    [Required, StringLength(90)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(700)]
    public string Description { get; set; } = string.Empty;

    [StringLength(700)]
    public string ImageUrl { get; set; } = string.Empty;

    [Range(1, 5)]
    public int StarRating { get; set; } = 4;

    [Range(0, 5)]
    public double GuestRating { get; set; } = 4.5;

    [StringLength(400)]
    public string Amenities { get; set; } = string.Empty;

    public ICollection<Room> Rooms { get; set; } = new List<Room>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    [NotMapped]
    public string[] AmenityList => Amenities
        .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    public override string GetDisplayName()
    {
        return $"{Name}, {City}";
    }
}
