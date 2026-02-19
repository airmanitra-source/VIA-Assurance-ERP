using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ClientApp.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal? _currentUser;

    public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // 1. Check the local memory first (Interactive session state)
        if (_currentUser != null && _currentUser.Identity?.IsAuthenticated == true)
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        // 2. Check the HttpContext (Initial SSR / Cookie state)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User.Identity?.IsAuthenticated == true)
        {
            return Task.FromResult(new AuthenticationState(httpContext.User));
        }

        // 3. Otherwise return anonymous
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
    }

    public void SetAuthenticationState(ClaimsPrincipal user)
    {
        _currentUser = user;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void ClearAuthenticationState()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
