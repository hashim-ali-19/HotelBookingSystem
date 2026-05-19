using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models;

public class Room : BaseEntity
{
    [Required]
    public int HotelId { get; set; }

    public Hotel? Hotel { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string RoomNumberPrefix { get; set; } = string.Empty;

    public RoomType Type { get; set; } = RoomType.Standard;

    [Range(1, 12)]
    public int Capacity { get; set; } = 2;

    [Required, StringLength(60)]
    public string BedType { get; set; } = string.Empty;

    [Range(1000, 500000)]
    public decimal PricePerNight { get; set; }

    [Range(0, 60)]
    public decimal DiscountPercent { get; set; }

    [Range(1, 50)]
    public int TotalUnits { get; set; } = 1;

    [Required, StringLength(500)]
    public string Amenities { get; set; } = string.Empty;

    [StringLength(700)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(180)]
    public string Highlight { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [NotMapped]
    public decimal FinalPrice => PricePerNight - (PricePerNight * DiscountPercent / 100);

    [NotMapped]
    public string[] AmenityList => Amenities
        .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    public override string GetDisplayName()
    {
        return $"{Hotel?.Name ?? "Hotel"} - {Name}";
    }
}
