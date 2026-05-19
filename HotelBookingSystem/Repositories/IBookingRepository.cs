using HotelBookingSystem.Models;

namespace HotelBookingSystem.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<List<Booking>> GetBookingsForUserAsync(int userId);

    Task<List<Booking>> GetBookingsForAdminAsync();

    Task<Booking?> GetBookingDetailsAsync(int id);
}
