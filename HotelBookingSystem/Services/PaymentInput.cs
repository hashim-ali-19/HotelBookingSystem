using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.Services;

public class PaymentInput : IValidatableObject
{
    public PaymentMethod Method { get; set; } = PaymentMethod.Card;

    [Required, StringLength(80)]
    public string PayerName { get; set; } = string.Empty;

    [StringLength(80)]
    public string CardHolderName { get; set; } = string.Empty;

    [StringLength(19)]
    public string CardNumber { get; set; } = string.Empty;

    [StringLength(5)]
    public string Expiry { get; set; } = string.Empty;

    [StringLength(4)]
    public string Cvv { get; set; } = string.Empty;

    [StringLength(120)]
    public string BillingAddress { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Method != PaymentMethod.Card)
        {
            yield break;
        }

        if (string.IsNullOrWhiteSpace(CardHolderName))
        {
            yield return new ValidationResult("Card holder name is required.", new[] { nameof(CardHolderName) });
        }

        var digits = new string(CardNumber.Where(char.IsDigit).ToArray());
        if (digits.Length is < 13 or > 19)
        {
            yield return new ValidationResult("Enter a valid card number.", new[] { nameof(CardNumber) });
        }

        if (!IsValidExpiry(Expiry))
        {
            yield return new ValidationResult("Expiry must be valid and in MM/YY format.", new[] { nameof(Expiry) });
        }

        if (Cvv.Length is < 3 or > 4 || !Cvv.All(char.IsDigit))
        {
            yield return new ValidationResult("CVV must be 3 or 4 digits.", new[] { nameof(Cvv) });
        }

        if (string.IsNullOrWhiteSpace(BillingAddress))
        {
            yield return new ValidationResult("Billing address is required for card payments.", new[] { nameof(BillingAddress) });
        }
    }

    private static bool IsValidExpiry(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 5 || value[2] != '/')
        {
            return false;
        }

        if (!int.TryParse(value[..2], out var month) || !int.TryParse(value[3..], out var year))
        {
            return false;
        }

        if (month is < 1 or > 12)
        {
            return false;
        }

        var expiryDate = new DateTime(2000 + year, month, 1).AddMonths(1).AddDays(-1);
        return expiryDate >= DateTime.Today;
    }
}
