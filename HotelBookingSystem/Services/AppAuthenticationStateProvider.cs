using Microsoft.AspNetCore.Components.Authorization;

namespace HotelBookingSystem.Services;

public class AppAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly AuthService _authService;

    public AppAuthenticationStateProvider(AuthService authService)
    {
        _authService = authService;
        _authService.AuthStateChanged += NotifyChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_authService.ToClaimsPrincipal()));
    }

    public void Dispose()
    {
        _authService.AuthStateChanged -= NotifyChanged;
    }

    private void NotifyChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
