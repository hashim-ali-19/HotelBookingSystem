using HotelBookingSystem.Models;
using HotelBookingSystem.Repositories;

namespace HotelBookingSystem.Services;

public class RoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public Task<List<Hotel>> GetHotelsAsync()
    {
        return _roomRepository.GetHotelsAsync();
    }

    public Task<List<Room>> GetRoomsAsync()
    {
        return _roomRepository.GetAllAsync();
    }

    public Task<List<Room>> SearchAvailableRoomsAsync(RoomSearchFilter filter)
    {
        return _roomRepository.SearchAvailableRoomsAsync(filter);
    }

    public Task<Room?> GetRoomAsync(int id)
    {
        return _roomRepository.GetByIdAsync(id);
    }

    public Task<int> GetAvailableUnitsAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        return _roomRepository.GetAvailableUnitsAsync(roomId, checkIn, checkOut);
    }

    public Task<List<Review>> GetRecentReviewsAsync(int count = 6)
    {
        return _roomRepository.GetRecentReviewsAsync(count);
    }

    public async Task<ServiceResult> SaveRoomAsync(Room room)
    {
        if (room.PricePerNight <= 0 || room.TotalUnits <= 0)
        {
            return new ServiceResult(false, "Price and total units must be greater than zero.");
        }

        if (room.Id == 0)
        {
            await _roomRepository.AddAsync(room);
            return new ServiceResult(true, "Room package added successfully.");
        }

        await _roomRepository.UpdateAsync(room);
        return new ServiceResult(true, "Room package updated successfully.");
    }

    public async Task<ServiceResult> DeleteRoomAsync(int roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room is null)
        {
            return new ServiceResult(false, "Room package not found.");
        }

        room.IsActive = false;
        await _roomRepository.UpdateAsync(room);
        return new ServiceResult(true, "Room package deactivated.");
    }

    public async Task<ServiceResult> AddReviewAsync(Review review)
    {
        if (review.Rating < 1 || review.Rating > 5)
        {
            return new ServiceResult(false, "Rating must be between 1 and 5.");
        }

        await _roomRepository.AddReviewAsync(review);
        return new ServiceResult(true, "Review submitted successfully.");
    }
}
