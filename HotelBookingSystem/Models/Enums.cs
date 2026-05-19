namespace HotelBookingSystem.Models;

public enum UserRole
{
    Guest = 1,
    Admin = 2
}

public enum RoomType
{
    Standard = 1,
    Deluxe = 2,
    Suite = 3,
    Family = 4,
    Penthouse = 5
}

public enum BookingStatus
{
    Pending = 1,
    Confirmed = 2,
    CheckedIn = 3,
    Completed = 4,
    Cancelled = 5
}

public enum PaymentStatus
{
    Unpaid = 1,
    Paid = 2,
    Refunded = 3
}

public enum PaymentMethod
{
    Cash = 1,
    Card = 2,
    BankTransfer = 3
}
