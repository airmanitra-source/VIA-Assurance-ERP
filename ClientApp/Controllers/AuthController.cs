using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FileTable.Infrastructure.Identities;

namespace ClientApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            return Ok(user.UserName);
        }
        return Ok(null);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpGet("/logout")]
    public async Task<IActionResult> PerformLogout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/login");
    }
}
