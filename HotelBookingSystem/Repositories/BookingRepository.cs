using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Repositories;

public class BookingRepository : EfRepository<Booking>, IBookingRepository
{
    public BookingRepository(HotelBookingDbContext dbContext)
        : base(dbContext)
    {
    }

    public override Task<List<Booking>> GetAllAsync()
    {
        return GetBookingsForAdminAsync();
    }

    public Task<List<Booking>> GetBookingsForUserAsync(int userId)
    {
        return WithDetails()
            .Where(booking => booking.UserId == userId)
            .OrderByDescending(booking => booking.CreatedAt)
            .ToListAsync();
    }

    public Task<List<Booking>> GetBookingsForAdminAsync()
    {
        return WithDetails()
            .OrderByDescending(booking => booking.CreatedAt)
            .ToListAsync();
    }

    public Task<Booking?> GetBookingDetailsAsync(int id)
    {
        return WithDetails().FirstOrDefaultAsync(booking => booking.Id == id);
    }

    private IQueryable<Booking> WithDetails()
    {
        return DbContext.Bookings
            .Include(booking => booking.User)
            .Include(booking => booking.Room)
                .ThenInclude(room => room!.Hotel)
            .Include(booking => booking.Payments);
    }
}
