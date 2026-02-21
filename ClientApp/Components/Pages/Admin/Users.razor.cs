using ClientApp.Services;
using FileTable.Infrastructure.Identities;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Admin
{
    public partial class Users : ComponentBase
    {
        // --- Injections ---
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected UserManagementService UserManagementService { get; set; } = default!;

        // --- State (alphabetically sorted, Id fields first) ---
        protected List<string> allRoles = new();
        protected bool isLoading = true;
        protected bool showRoleModal = false;
        protected ApplicationUser? selectedUser;
        protected HashSet<string> selectedUserRoles = new();
        protected List<ApplicationUser> users = new();
        protected Dictionary<string, IList<string>> userRoles = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            allRoles = UserManagementService.GetAllRoles();
        }

        #region Private Methods

        private async Task LoadDataAsync()
        {
            isLoading = true;
            try
            {
                users = await UserManagementService.GetAllUsersAsync();
                userRoles.Clear();
                foreach (var user in users)
                {
                    var roles = await UserManagementService.GetUserRolesAsync(user);
                    userRoles[user.Id] = roles;
                }
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task OpenRoleModalAsync(ApplicationUser user)
        {
            selectedUser = user;
            var roles = await UserManagementService.GetUserRolesAsync(user);
            selectedUserRoles = new HashSet<string>(roles);
            showRoleModal = true;
        }

        private void CloseModal()
        {
            showRoleModal = false;
            selectedUser = null;
            selectedUserRoles.Clear();
        }

        private async Task ToggleRoleAsync(string role, object? value)
        {
            if (selectedUser == null || value is not bool isChecked) return;

            try
            {
                if (isChecked)
                {
                    await UserManagementService.AddRoleAsync(selectedUser, role);
                    selectedUserRoles.Add(role);
                }
                else
                {
                    await UserManagementService.RemoveRoleAsync(selectedUser, role);
                    selectedUserRoles.Remove(role);
                }

                var roles = await UserManagementService.GetUserRolesAsync(selectedUser);
                userRoles[selectedUser.Id] = roles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating role: {ex.Message}");
            }
        }

        #endregion
    }
}
