using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Repositories;

public class RoomRepository : EfRepository<Room>, IRoomRepository
{
    public RoomRepository(HotelBookingDbContext dbContext)
        : base(dbContext)
    {
    }

    public override async Task<List<Room>> GetAllAsync()
    {
        var rooms = await DbContext.Rooms
            .Include(room => room.Hotel)
            .Include(room => room.Bookings)
            .AsNoTracking()
            .OrderBy(room => room.Hotel!.City)
            .ToListAsync();

        return rooms
            .OrderBy(room => room.Hotel?.City)
            .ThenBy(room => room.FinalPrice)
            .ToList();
    }

    public override Task<Room?> GetByIdAsync(int id)
    {
        return DbContext.Rooms
            .Include(room => room.Hotel)
            .Include(room => room.Bookings)
            .FirstOrDefaultAsync(room => room.Id == id);
    }

    public override async Task UpdateAsync(Room entity)
    {
        var existing = await DbContext.Rooms.FirstOrDefaultAsync(room => room.Id == entity.Id);
        if (existing is null)
        {
            return;
        }

        existing.HotelId = entity.HotelId;
        existing.Name = entity.Name;
        existing.RoomNumberPrefix = entity.RoomNumberPrefix;
        existing.Type = entity.Type;
        existing.Capacity = entity.Capacity;
        existing.BedType = entity.BedType;
        existing.PricePerNight = entity.PricePerNight;
        existing.DiscountPercent = entity.DiscountPercent;
        existing.TotalUnits = entity.TotalUnits;
        existing.Amenities = entity.Amenities;
        existing.Highlight = entity.Highlight;
        existing.ImageUrl = entity.ImageUrl;
        existing.IsActive = entity.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await DbContext.SaveChangesAsync();
    }

    public Task<List<Hotel>> GetHotelsAsync()
    {
        return DbContext.Hotels
            .Include(hotel => hotel.Rooms)
            .Include(hotel => hotel.Reviews)
            .AsNoTracking()
            .OrderByDescending(hotel => hotel.GuestRating)
            .ToListAsync();
    }

    public Task<Hotel?> GetHotelAsync(int hotelId)
    {
        return DbContext.Hotels
            .Include(hotel => hotel.Rooms)
            .Include(hotel => hotel.Reviews)
            .FirstOrDefaultAsync(hotel => hotel.Id == hotelId);
    }

    public async Task<List<Room>> SearchAvailableRoomsAsync(RoomSearchFilter filter)
    {
        var query = DbContext.Rooms
            .Include(room => room.Hotel)
            .Include(room => room.Bookings)
            .Where(room => room.IsActive && room.Capacity >= filter.Guests);

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            var city = filter.City.Trim().ToLower();
            query = query.Where(room => room.Hotel!.City.ToLower() == city);
        }

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            query = query.Where(room =>
                room.Name.Contains(filter.Keyword) ||
                room.Amenities.Contains(filter.Keyword) ||
                room.Hotel!.Name.Contains(filter.Keyword));
        }

        if (filter.Type is not null)
        {
            query = query.Where(room => room.Type == filter.Type);
        }

        var rooms = await query
            .AsNoTracking()
            .ToListAsync();

        return rooms
            .Where(room => filter.MaxPrice is null || room.FinalPrice <= filter.MaxPrice.Value)
            .Where(room => CalculateAvailableUnits(room, filter.CheckIn, filter.CheckOut) > 0)
            .OrderBy(room => room.FinalPrice)
            .ToList();
    }

    public async Task<int> GetAvailableUnitsAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var room = await DbContext.Rooms
            .Include(item => item.Bookings)
            .FirstOrDefaultAsync(item => item.Id == roomId);

        return room is null ? 0 : CalculateAvailableUnits(room, checkIn, checkOut);
    }

    public Task<List<Review>> GetRecentReviewsAsync(int count = 6)
    {
        return DbContext.Reviews
            .Include(review => review.Hotel)
            .Where(review => review.IsApproved)
            .AsNoTracking()
            .OrderByDescending(review => review.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task AddReviewAsync(Review review)
    {
        DbContext.Reviews.Add(review);
        await DbContext.SaveChangesAsync();
    }

    private static int CalculateAvailableUnits(Room room, DateTime checkIn, DateTime checkOut)
    {
        var overlappingBookings = room.Bookings.Count(booking =>
            booking.Status != BookingStatus.Cancelled &&
            booking.CheckIn.Date < checkOut.Date &&
            checkIn.Date < booking.CheckOut.Date);

        return Math.Max(0, room.TotalUnits - overlappingBookings);
    }
}
