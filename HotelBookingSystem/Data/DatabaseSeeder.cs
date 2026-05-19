using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();

        await db.Database.EnsureCreatedAsync();

        await CleanupDemoGuestDataAsync(db);

        if (await db.Hotels.AnyAsync())
        {
            await ApplyRoomImageUpdatesAsync(db);
            return;
        }

        var admin = new User
        {
            FullName = "Project Admin",
            Email = "admin@aurastay.com",
            Phone = "03001234567",
            PasswordHash = AuthService.HashPassword("admin123"),
            Role = UserRole.Admin
        };

        db.Users.Add(admin);

        var hotels = new List<Hotel>
        {
            new()
            {
                Name = "Astra Grand Lahore",
                City = "Lahore",
                Address = "Mall Road, Lahore",
                StarRating = 5,
                GuestRating = 4.8,
                Description = "A premium city hotel with executive lounges, rooftop dining, and fast access to Lahore business districts.",
                Amenities = "Airport pickup,Rooftop restaurant,Spa,Conference halls,Valet parking",
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=1000&q=80"
            },
            new()
            {
                Name = "Seaview Crown Karachi",
                City = "Karachi",
                Address = "Clifton Beach, Karachi",
                StarRating = 5,
                GuestRating = 4.7,
                Description = "A coastal business resort with sea-facing suites, banquet halls, and family-friendly recreation.",
                Amenities = "Sea view,Infinity pool,Banquet hall,Kids club,24/7 room service",
                ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?auto=format&fit=crop&w=1000&q=80"
            },
            new()
            {
                Name = "Margalla Executive Islamabad",
                City = "Islamabad",
                Address = "Blue Area, Islamabad",
                StarRating = 4,
                GuestRating = 4.6,
                Description = "A quiet executive stay near corporate offices, embassies, and Margalla hiking routes.",
                Amenities = "Mountain view,Business center,Smart rooms,Fitness studio,Private parking",
                ImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?auto=format&fit=crop&w=1000&q=80"
            },
            new()
            {
                Name = "Hunza Vista Resort",
                City = "Hunza",
                Address = "Karimabad, Hunza Valley",
                StarRating = 4,
                GuestRating = 4.9,
                Description = "A boutique mountain resort with panoramic valley rooms, guided tours, and local cuisine.",
                Amenities = "Valley view,Bonfire deck,Tour desk,Organic breakfast,Heated rooms",
                ImageUrl = "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?auto=format&fit=crop&w=1000&q=80"
            }
        };

        db.Hotels.AddRange(hotels);
        await db.SaveChangesAsync();

        var rooms = new List<Room>
        {
            NewRoom(hotels[0], "Executive King", "LG-EX", RoomType.Deluxe, 2, "King bed", 24500, 10, 8, "City skyline,Smart TV,Mini bar,Work desk,Breakfast", "Corporate-ready room with executive lounge access.", RoomImages["LG-EX"]),
            NewRoom(hotels[0], "Presidential Suite", "LG-PS", RoomType.Penthouse, 4, "King bed + lounge", 72000, 8, 2, "Jacuzzi,Butler service,Lounge,Premium breakfast,Private check-in", "Signature suite for VIP guests and families.", RoomImages["LG-PS"]),
            NewRoom(hotels[0], "Family Comfort", "LG-FM", RoomType.Family, 5, "Two queen beds", 33000, 12, 5, "Connecting rooms,Kids amenities,Smart TV,Breakfast,Parking", "Spacious family setup near major Lahore attractions.", RoomImages["LG-FM"]),

            NewRoom(hotels[1], "Ocean Deluxe", "KC-OD", RoomType.Deluxe, 3, "King bed", 28500, 15, 10, "Sea view,Balcony,Pool access,Breakfast,Mini bar", "Best seller with a direct Arabian Sea view.", RoomImages["KC-OD"]),
            NewRoom(hotels[1], "Business Twin", "KC-BT", RoomType.Standard, 2, "Twin beds", 18500, 5, 12, "Fast Wi-Fi,Work desk,Breakfast,Laundry,Airport desk", "Practical room for short corporate trips.", RoomImages["KC-BT"]),
            NewRoom(hotels[1], "Royal Suite", "KC-RS", RoomType.Suite, 4, "King bed + sofa bed", 52000, 10, 3, "Private balcony,Lounge,Club access,Jacuzzi,Sea view", "Large suite for events, anniversaries, and premium stays.", RoomImages["KC-RS"]),

            NewRoom(hotels[2], "Diplomat Studio", "IS-DS", RoomType.Standard, 2, "Queen bed", 16500, 0, 14, "Smart lock,Desk,Mountain glimpse,Breakfast,Parking", "Compact, quiet room for embassy and business visits.", RoomImages["IS-DS"]),
            NewRoom(hotels[2], "Margalla View Suite", "IS-MV", RoomType.Suite, 3, "King bed", 38500, 7, 4, "Mountain view,Lounge,Premium breakfast,Mini bar,Airport pickup", "Premium suite facing the Margalla hills.", RoomImages["IS-MV"]),
            NewRoom(hotels[2], "Boardroom Residence", "IS-BR", RoomType.Family, 6, "Two bedrooms", 45500, 9, 3, "Kitchenette,Meeting table,Two baths,Breakfast,Secure parking", "Apartment-style stay with private meeting space.", RoomImages["IS-BR"]),

            NewRoom(hotels[3], "Valley Classic", "HZ-VC", RoomType.Standard, 2, "Queen bed", 14200, 0, 9, "Heated room,Valley window,Breakfast,Tour desk,Tea station", "Warm, simple room with an open valley view.", RoomImages["HZ-VC"]),
            NewRoom(hotels[3], "Panorama Suite", "HZ-PS", RoomType.Suite, 3, "King bed", 31500, 6, 4, "Panorama balcony,Bonfire access,Heated floors,Breakfast,Tour guide", "A scenic suite designed for photography and comfort.", RoomImages["HZ-PS"]),
            NewRoom(hotels[3], "Family Chalet", "HZ-FC", RoomType.Family, 6, "Two queen beds", 39800, 5, 5, "Private deck,Heated rooms,Local breakfast,Tour desk,Bonfire", "Mountain chalet for families and group trips.", RoomImages["HZ-FC"])
        };

        db.Rooms.AddRange(rooms);
        await db.SaveChangesAsync();

        db.Reviews.AddRange(
            NewReview(hotels[1], null, 4, "Loved the sea view and breakfast. Admin team handled changes quickly."),
            NewReview(hotels[2], null, 5, "Quiet, professional, and perfect for a business trip."),
            NewReview(hotels[3], null, 5, "The valley suite view was unforgettable.")
        );

        await db.SaveChangesAsync();
    }

    private static readonly Dictionary<string, string> RoomImages = new()
    {
        ["LG-EX"] = "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=1100&q=80",
        ["LG-PS"] = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=1100&q=80",
        ["LG-FM"] = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?auto=format&fit=crop&w=1100&q=80",
        ["KC-OD"] = "https://images.unsplash.com/photo-1596394516093-501ba68a0ba6?auto=format&fit=crop&w=1100&q=80",
        ["KC-BT"] = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?auto=format&fit=crop&w=1100&q=80",
        ["KC-RS"] = "https://images.unsplash.com/photo-1618221195710-dd6b41faaea6?auto=format&fit=crop&w=1100&q=80",
        ["IS-DS"] = "https://images.unsplash.com/photo-1611892440504-42a792e24d32?auto=format&fit=crop&w=1100&q=80",
        ["IS-MV"] = "https://images.unsplash.com/photo-1591088398332-8a7791972843?auto=format&fit=crop&w=1100&q=80",
        ["IS-BR"] = "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?auto=format&fit=crop&w=1100&q=80",
        ["HZ-VC"] = "https://images.unsplash.com/photo-1578681041175-9717c5c7f585?auto=format&fit=crop&w=1100&q=80",
        ["HZ-PS"] = "https://images.unsplash.com/photo-1602002418082-a4443e081dd1?auto=format&fit=crop&w=1100&q=80",
        ["HZ-FC"] = "https://images.unsplash.com/photo-1529290130-4ca3753253ae?auto=format&fit=crop&w=1100&q=80"
    };

    private static async Task ApplyRoomImageUpdatesAsync(HotelBookingDbContext db)
    {
        var rooms = await db.Rooms.ToListAsync();
        var changed = false;

        foreach (var room in rooms)
        {
            if (RoomImages.TryGetValue(room.RoomNumberPrefix, out var imageUrl) && room.ImageUrl != imageUrl)
            {
                room.ImageUrl = imageUrl;
                changed = true;
            }
        }

        if (changed)
        {
            await db.SaveChangesAsync();
        }
    }

    private static async Task CleanupDemoGuestDataAsync(HotelBookingDbContext db)
    {
        var demoBookings = await db.Bookings
            .Where(booking =>
                booking.BookingCode == "AS-240501" ||
                booking.BookingCode == "AS-240502" ||
                booking.BookingCode == "AS-240503" ||
                booking.BookingCode == "AS-240504" ||
                booking.BookingCode == "AS-240505" ||
                booking.GuestEmail == "guest@aurastay.com" ||
                booking.GuestEmail == "hassan.raza@example.com" ||
                booking.GuestEmail == "ayesha.khan@example.com" ||
                booking.GuestEmail == "bilal.ahmed@example.com")
            .ToListAsync();

        if (demoBookings.Count > 0)
        {
            var demoBookingIds = demoBookings.Select(booking => booking.Id).ToList();
            var demoPayments = await db.Payments
                .Where(payment => demoBookingIds.Contains(payment.BookingId))
                .ToListAsync();

            db.Payments.RemoveRange(demoPayments);
            db.Bookings.RemoveRange(demoBookings);
        }

        var demoGuest = await db.Users.FirstOrDefaultAsync(user => user.Email == "guest@aurastay.com");
        if (demoGuest is not null)
        {
            var demoReviews = await db.Reviews
                .Where(review => review.UserId == demoGuest.Id)
                .ToListAsync();

            db.Reviews.RemoveRange(demoReviews);
            db.Users.Remove(demoGuest);
        }

        await db.SaveChangesAsync();
    }

    private static Room NewRoom(Hotel hotel, string name, string prefix, RoomType type, int capacity, string bedType, decimal price, decimal discount, int units, string amenities, string highlight, string imageUrl)
    {
        return new Room
        {
            HotelId = hotel.Id,
            Name = name,
            RoomNumberPrefix = prefix,
            Type = type,
            Capacity = capacity,
            BedType = bedType,
            PricePerNight = price,
            DiscountPercent = discount,
            TotalUnits = units,
            Amenities = amenities,
            Highlight = highlight,
            ImageUrl = imageUrl
        };
    }

    private static Review NewReview(Hotel hotel, User? user, int rating, string comment)
    {
        return new Review
        {
            HotelId = hotel.Id,
            UserId = user?.Id,
            ReviewerName = user?.FullName ?? "Verified Traveler",
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow.AddDays(-rating)
        };
    }
}
