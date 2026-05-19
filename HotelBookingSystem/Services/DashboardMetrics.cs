namespace HotelBookingSystem.Services;

public class DashboardMetrics
{
    public int TotalHotels { get; set; }

    public int TotalRooms { get; set; }

    public int ActiveBookings { get; set; }

    public int TotalGuests { get; set; }

    public decimal Revenue { get; set; }

    public decimal EstimatedProfit { get; set; }

    public decimal PendingPayments { get; set; }

    public double OccupancyRate { get; set; }

    public double AverageRating { get; set; }

    public int PaidBookings { get; set; }

    public List<HotelPerformance> HotelPerformance { get; set; } = new();
}

public class HotelPerformance
{
    public string HotelName { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public int Bookings { get; set; }

    public decimal Revenue { get; set; }

    public double Rating { get; set; }
}
