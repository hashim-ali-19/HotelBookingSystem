using HotelBookingSystem.Models;
using HotelBookingSystem.Services;

namespace HotelBookingSystem.Repositories;

public interface IRoomRepository : IRepository<Room>
{
    Task<List<Hotel>> GetHotelsAsync();

    Task<Hotel?> GetHotelAsync(int hotelId);

    Task<List<Room>> SearchAvailableRoomsAsync(RoomSearchFilter filter);

    Task<int> GetAvailableUnitsAsync(int roomId, DateTime checkIn, DateTime checkOut);

    Task<List<Review>> GetRecentReviewsAsync(int count = 6);

    Task AddReviewAsync(Review review);
}
