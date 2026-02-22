using FileTable.Infrastructure.Identities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ClientApp.Services;

public class AuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly CustomAuthenticationStateProvider _authStateProvider;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager,
        CustomAuthenticationStateProvider authStateProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authStateProvider = authStateProvider;
    }

    public event Action? OnAuthStateChanged;

    public async Task<(bool Success, string? Message)> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return (false, "Invalid username or password.");
        }

        // Check if user needs to reset password (except for developers)
        var userRoles = await _userManager.GetRolesAsync(user);
        var isDeveloper = userRoles.Contains("developer");
        
        if (!isDeveloper && !user.InitialPasswordResetCompleted)
        {
            // User must reset password first
            return (false, "Your account requires initial password setup. Please check your email for the password reset link.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        
        if (result.Succeeded)
        {
            // Fetch roles and build role claims BEFORE signing in
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
            
            // Sign in with role claims and EntrepriseId baked into the cookie
            var additionalClaims = new List<Claim>(roleClaims)
            {
                new Claim("EntrepriseId", user.EntrepriseId.ToString())
            };
            await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, additionalClaims);
            
            // Create claims for the custom authentication state provider
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("EntrepriseId", user.EntrepriseId.ToString())
            };
            claims.AddRange(roleClaims);

            var claimsIdentity = new ClaimsIdentity(claims, "ApplicationAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            
            _authStateProvider.SetAuthenticationState(claimsPrincipal);
            OnAuthStateChanged?.Invoke();
            return (true, null);
        }

        return (false, "Invalid username or password.");
    }

    public async Task<IdentityResult> RegisterAsync(long entrepriseId, string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EntrepriseId = entrepriseId,
            Id = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, password);
        
        if (result.Succeeded)
        {
            // Fetch roles and build role claims
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
            
            // Sign in with role claims and EntrepriseId baked into the cookie
            var additionalClaims = new List<Claim>(roleClaims)
            {
                new Claim("EntrepriseId", user.EntrepriseId.ToString())
            };
            await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, additionalClaims);
            
            // Create claims for the custom authentication state provider
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("EntrepriseId", user.EntrepriseId.ToString())
            };
            claims.AddRange(roleClaims);

            var claimsIdentity = new ClaimsIdentity(claims, "ApplicationAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            
            _authStateProvider.SetAuthenticationState(claimsPrincipal);
            OnAuthStateChanged?.Invoke();
        }

        return result;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        _authStateProvider.ClearAuthenticationState();
        OnAuthStateChanged?.Invoke();
    }

    public bool IsAuthenticated()
    {
        var authState = _authStateProvider.GetAuthenticationStateAsync().Result;
        return authState.User?.Identity?.IsAuthenticated ?? false;
    }

    public string? GetCurrentUserEmail()
    {
        var authState = _authStateProvider.GetAuthenticationStateAsync().Result;
        return authState.User?.Identity?.Name;
    }

    public string? GetCurrentUserId()
    {
        var authState = _authStateProvider.GetAuthenticationStateAsync().Result;
        return authState.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public long? GetCurrentEntrepriseId()
    {
        var authState = _authStateProvider.GetAuthenticationStateAsync().Result;
        var value = authState.User?.FindFirst("EntrepriseId")?.Value;
        return value != null ? long.Parse(value) : null;
    }
}
