using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Repositories;

namespace HotelBookingSystem.Services;

public class PaymentService
{
    private readonly HotelBookingDbContext _dbContext;
    private readonly IBookingRepository _bookingRepository;

    public PaymentService(HotelBookingDbContext dbContext, IBookingRepository bookingRepository)
    {
        _dbContext = dbContext;
        _bookingRepository = bookingRepository;
    }

    public async Task<ServiceResult> PayBookingAsync(int bookingId, PaymentInput input)
    {
        var booking = await _bookingRepository.GetBookingDetailsAsync(bookingId);
        if (booking is null)
        {
            return new ServiceResult(false, "Booking not found.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return new ServiceResult(false, "Cancelled bookings cannot be paid.");
        }

        if (booking.PaymentStatus == PaymentStatus.Paid)
        {
            return new ServiceResult(false, "This booking is already paid.");
        }

        var payment = new Payment
        {
            BookingId = booking.Id,
            PayerName = input.PayerName.Trim(),
            Amount = booking.TotalAmount,
            Method = input.Method,
            Status = PaymentStatus.Paid,
            TransactionReference = BuildTransactionReference(input),
            PaidAt = DateTime.UtcNow
        };

        booking.PaymentStatus = PaymentStatus.Paid;
        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAt = DateTime.UtcNow;

        _dbContext.Payments.Add(payment);
        _dbContext.Bookings.Update(booking);
        await _dbContext.SaveChangesAsync();

        return new ServiceResult(true, $"Payment captured: {payment.TransactionReference}.");
    }

    private static string BuildTransactionReference(PaymentInput input)
    {
        if (input.Method != PaymentMethod.Card)
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
        }

        var digits = new string(input.CardNumber.Where(char.IsDigit).ToArray());
        var lastFour = digits.Length >= 4 ? digits[^4..] : "0000";
        return $"CARD-{lastFour}-{DateTime.UtcNow:HHmmss}";
    }
}
