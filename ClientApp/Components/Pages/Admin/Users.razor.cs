using ClientApp.Components.Shared;
using ClientApp.Services;
using FileTable.Infrastructure.Identities;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Admin
{
    public partial class Users : AuthenticatedComponentBase
    {
        // --- Injections ---
        [Inject] protected UserManagementService UserManagementService { get; set; } = default!;

        // --- State (alphabetically sorted, Id fields first) ---
        protected List<string> allRoles = new();
        protected bool isLoading = true;
        protected bool isProcessing = false;
        protected bool showAddUserModal = false;
        protected bool showConfirmDeleteModal = false;
        protected bool showRoleModal = false;
        protected string addUserConfirmPassword = string.Empty;
        protected string addUserEmail = string.Empty;
        protected string addUserPassword = string.Empty;
        protected string errorMessage = string.Empty;
        protected ApplicationUser? pendingDeleteUser;
        protected ApplicationUser? selectedUser;
        protected HashSet<string> selectedUserRoles = new();
        protected List<ApplicationUser> users = new();
        protected Dictionary<string, IList<string>> userRoles = new();

        protected override async Task OnInitializedAuthenticatedAsync()
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

        private void OpenAddUserModal()
        {
            errorMessage = string.Empty;
            addUserConfirmPassword = string.Empty;
            addUserEmail = string.Empty;
            addUserPassword = string.Empty;
            showAddUserModal = true;
        }

        private async Task CreateUserAsync()
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(addUserEmail) || string.IsNullOrWhiteSpace(addUserPassword))
            {
                errorMessage = "Email, password are required.";
                return;
            }

            if (addUserPassword != addUserConfirmPassword)
            {
                errorMessage = "Passwords do not match. Please try again.";
                return;
            }

            if (addUserPassword.Length < 6)
            {
                errorMessage = "Password must be at least 6 characters long.";
                return;
            }

            if (!CurrentEnterpriseId.HasValue)
            {
                errorMessage = "Current enterprise information is not available.";
                return;
            }

            isProcessing = true;
            try
            {
                var existingUser = await UserManagementService.FindUserByEmailAsync(addUserEmail);
                if (existingUser != null)
                {
                    errorMessage = "User with this email already exists.";
                    return;
                }

                var newUser = new ApplicationUser
                {
                    UserName = addUserEmail,
                    Email = addUserEmail,
                    EntrepriseId = CurrentEnterpriseId.Value
                };

                var result = await UserManagementService.CreateUserAsync(newUser, addUserPassword);
                if (result.Succeeded)
                {
                    await LoadDataAsync();
                    CloseAddUserModal();
                }
                else
                {
                    errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error creating user: {ex.Message}";
                Console.WriteLine($"Error creating user: {ex.Message}");
            }
            finally
            {
                isProcessing = false;
            }
        }

        private void CloseAddUserModal()
        {
            showAddUserModal = false;
            addUserConfirmPassword = string.Empty;
            addUserEmail = string.Empty;
            addUserPassword = string.Empty;
            errorMessage = string.Empty;
        }

        private async Task OpenRoleModalAsync(ApplicationUser user)
        {
            selectedUser = user;
            var roles = await UserManagementService.GetUserRolesAsync(user);
            selectedUserRoles = new HashSet<string>(roles);
            showRoleModal = true;
        }

        private void CloseRoleModal()
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

        private void OpenConfirmDeleteModal(ApplicationUser user)
        {
            pendingDeleteUser = user;
            showConfirmDeleteModal = true;
        }

        private void CloseConfirmDeleteModal()
        {
            showConfirmDeleteModal = false;
            pendingDeleteUser = null;
        }

        private async Task ConfirmInactivateUserAsync()
        {
            if (pendingDeleteUser == null) return;

            isProcessing = true;
            try
            {
                var result = await UserManagementService.DeleteUserAsync(pendingDeleteUser);
                if (result.Succeeded)
                {
                    await LoadDataAsync();
                    CloseConfirmDeleteModal();
                }
                else
                {
                    errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error inactivating user: {ex.Message}";
                Console.WriteLine($"Error inactivating user: {ex.Message}");
            }
            finally
            {
                isProcessing = false;
            }
        }

        #endregion
    }
}
