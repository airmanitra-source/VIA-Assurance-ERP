using FileTable.Infrastructure.Identities;
using Microsoft.AspNetCore.Identity;

namespace ClientApp.Services;

public class UserManagementService
{
    private readonly ApplicationUserStore _userStore;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagementService(ApplicationUserStore userStore, UserManager<ApplicationUser> userManager)
    {
        _userStore = userStore;
        _userManager = userManager;
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        var users = await _userStore.GetUsersAsync(CancellationToken.None);
        return users.ToList();
    }

    public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> AddRoleAsync(ApplicationUser user, string role)
    {
        return await _userManager.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> RemoveRoleAsync(ApplicationUser user, string role)
    {
        return await _userManager.RemoveFromRoleAsync(user, role);
    }

    public List<string> GetAllRoles()
    {
        return new List<string> { "directeur", "RH", "auditeur", "admin", "developer", "employee" };
    }
}
