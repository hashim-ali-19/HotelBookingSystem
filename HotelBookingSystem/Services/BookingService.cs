using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Services;

public class BookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly HotelBookingDbContext _dbContext;

    public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository, HotelBookingDbContext dbContext)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _dbContext = dbContext;
    }

    public Task<List<Booking>> GetBookingsForUserAsync(int userId)
    {
        return _bookingRepository.GetBookingsForUserAsync(userId);
    }

    public Task<List<Booking>> GetBookingsForAdminAsync()
    {
        return _bookingRepository.GetBookingsForAdminAsync();
    }

    public async Task<(ServiceResult Result, Booking? Booking)> CreateBookingAsync(BookingRequest request, User? user)
    {
        if (user is null)
        {
            return (new ServiceResult(false, "Please login before creating a booking."), null);
        }

        if (request.RoomUnits != 1)
        {
            return (new ServiceResult(false, "Please select exactly one room unit before confirming."), null);
        }

        var room = await _roomRepository.GetByIdAsync(request.RoomId);
        if (room is null)
        {
            return (new ServiceResult(false, "Selected room was not found."), null);
        }

        if (request.Guests > room.Capacity)
        {
            return (new ServiceResult(false, "Guest count is higher than this room capacity."), null);
        }

        var availableUnits = await _roomRepository.GetAvailableUnitsAsync(room.Id, request.CheckIn, request.CheckOut);
        if (availableUnits <= 0)
        {
            return (new ServiceResult(false, "This room package is sold out for selected dates."), null);
        }

        var nights = Math.Max(1, (request.CheckOut.Date - request.CheckIn.Date).Days);
        var booking = new Booking
        {
            UserId = user.Id,
            RoomId = room.Id,
            BookingCode = GenerateBookingCode(),
            GuestName = request.GuestName.Trim(),
            GuestEmail = request.GuestEmail.Trim().ToLowerInvariant(),
            GuestPhone = request.GuestPhone.Trim(),
            CheckIn = request.CheckIn.Date,
            CheckOut = request.CheckOut.Date,
            Guests = request.Guests,
            SpecialRequest = request.SpecialRequest.Trim(),
            TotalAmount = room.FinalPrice * nights,
            Status = BookingStatus.Pending,
            PaymentStatus = PaymentStatus.Unpaid
        };

        await _bookingRepository.AddAsync(booking);
        return (new ServiceResult(true, $"Booking {booking.BookingCode} created successfully."), booking);
    }

    public async Task<ServiceResult> CancelBookingAsync(int bookingId, User? user)
    {
        var booking = await _bookingRepository.GetBookingDetailsAsync(bookingId);
        if (booking is null)
        {
            return new ServiceResult(false, "Booking not found.");
        }

        if (user is null || (!IsOwner(user, booking) && user.Role != UserRole.Admin))
        {
            return new ServiceResult(false, "You are not allowed to cancel this booking.");
        }

        if (booking.Status is BookingStatus.Completed or BookingStatus.CheckedIn)
        {
            return new ServiceResult(false, "Checked-in or completed bookings cannot be cancelled.");
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;
        await _bookingRepository.UpdateAsync(booking);
        return new ServiceResult(true, "Booking cancelled successfully.");
    }

    public async Task<ServiceResult> UpdateBookingStatusAsync(int bookingId, BookingStatus status)
    {
        var booking = await _bookingRepository.GetBookingDetailsAsync(bookingId);
        if (booking is null)
        {
            return new ServiceResult(false, "Booking not found.");
        }

        booking.Status = status;
        booking.UpdatedAt = DateTime.UtcNow;
        await _bookingRepository.UpdateAsync(booking);
        return new ServiceResult(true, "Booking status updated.");
    }

    public async Task<DashboardMetrics> GetDashboardMetricsAsync()
    {
        var bookings = await _dbContext.Bookings
            .Include(booking => booking.Room)
                .ThenInclude(room => room!.Hotel)
            .AsNoTracking()
            .ToListAsync();

        var rooms = await _dbContext.Rooms.AsNoTracking().ToListAsync();
        var hotels = await _dbContext.Hotels.AsNoTracking().ToListAsync();
        var reviews = await _dbContext.Reviews.AsNoTracking().ToListAsync();

        var activeBookings = bookings
            .Where(booking => booking.Status != BookingStatus.Cancelled)
            .ToList();

        var paidBookings = activeBookings
            .Where(booking => booking.PaymentStatus == PaymentStatus.Paid)
            .ToList();

        var availableRoomNights = Math.Max(1, rooms.Sum(room => room.TotalUnits) * 30);
        var bookedRoomNights = activeBookings.Sum(booking => booking.Nights);

        return new DashboardMetrics
        {
            TotalHotels = hotels.Count,
            TotalRooms = rooms.Sum(room => room.TotalUnits),
            ActiveBookings = activeBookings.Count,
            TotalGuests = activeBookings.Sum(booking => booking.Guests),
            Revenue = paidBookings.Sum(booking => booking.TotalAmount),
            EstimatedProfit = Math.Round(paidBookings.Sum(booking => booking.TotalAmount) * 0.35m, 0),
            PendingPayments = activeBookings.Where(booking => booking.PaymentStatus == PaymentStatus.Unpaid).Sum(booking => booking.TotalAmount),
            OccupancyRate = Math.Round((double)bookedRoomNights / availableRoomNights * 100, 1),
            AverageRating = reviews.Count == 0 ? 0 : Math.Round(reviews.Average(review => review.Rating), 1),
            PaidBookings = paidBookings.Count,
            HotelPerformance = hotels
                .Select(hotel =>
                {
                    var hotelBookings = activeBookings.Where(booking => booking.Room?.HotelId == hotel.Id).ToList();
                    return new HotelPerformance
                    {
                        HotelName = hotel.Name,
                        City = hotel.City,
                        Bookings = hotelBookings.Count,
                        Revenue = hotelBookings.Where(booking => booking.PaymentStatus == PaymentStatus.Paid).Sum(booking => booking.TotalAmount),
                        Rating = hotel.GuestRating
                    };
                })
                .OrderByDescending(item => item.Revenue)
                .ToList()
        };
    }

    private static bool IsOwner(User user, Booking booking)
    {
        return booking.UserId == user.Id;
    }

    private static string GenerateBookingCode()
    {
        return $"AS-{DateTime.UtcNow:HHmmss}{Random.Shared.Next(100, 999)}";
    }
}
