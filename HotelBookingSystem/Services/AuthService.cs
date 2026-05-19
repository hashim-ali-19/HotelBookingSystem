using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Services;

public class AuthService
{
    private readonly HotelBookingDbContext _dbContext;

    public AuthService(HotelBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public event Action? AuthStateChanged;

    public User? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser is not null;

    public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;

    public async Task<ServiceResult> LoginAsync(LoginInput input, UserRole? requiredRole = null)
    {
        var normalizedEmail = input.Email.Trim().ToLowerInvariant();
        var passwordHash = HashPassword(input.Password);

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(item => item.Email.ToLower() == normalizedEmail && item.PasswordHash == passwordHash && item.IsActive);

        if (user is null)
        {
            return new ServiceResult(false, "Invalid email or password.");
        }

        if (requiredRole is not null && user.Role != requiredRole)
        {
            return new ServiceResult(false, requiredRole == UserRole.Admin
                ? "This portal is only for administration accounts."
                : "This portal is only for guest accounts. Use the admin portal for staff access.");
        }

        CurrentUser = user;
        AuthStateChanged?.Invoke();
        return new ServiceResult(true, $"Welcome back, {user.FullName}.");
    }

    public async Task<ServiceResult> RegisterAsync(RegisterInput input)
    {
        var normalizedEmail = input.Email.Trim().ToLowerInvariant();
        var exists = await _dbContext.Users.AnyAsync(user => user.Email.ToLower() == normalizedEmail);

        if (exists)
        {
            return new ServiceResult(false, "This email is already registered.");
        }

        var user = new User
        {
            FullName = input.FullName.Trim(),
            Email = normalizedEmail,
            Phone = input.Phone.Trim(),
            PasswordHash = HashPassword(input.Password),
            Role = UserRole.Guest
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        CurrentUser = user;
        AuthStateChanged?.Invoke();
        return new ServiceResult(true, "Account created successfully.");
    }

    public Task LogoutAsync()
    {
        CurrentUser = null;
        AuthStateChanged?.Invoke();
        return Task.CompletedTask;
    }

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        if (CurrentUser is null)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, CurrentUser.Id.ToString()),
            new Claim(ClaimTypes.Name, CurrentUser.FullName),
            new Claim(ClaimTypes.Email, CurrentUser.Email),
            new Claim(ClaimTypes.Role, CurrentUser.Role.ToString())
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "HotelBookingSession"));
    }

    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
