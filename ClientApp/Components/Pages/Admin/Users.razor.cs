using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using ClientApp.Services;
using FileTable.Infrastructure.Identities;
using FileTable.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ClientApp.Components.Pages.Admin
{
    public partial class Users : AuthenticatedComponentBase
    {
        // --- Injections ---
        [Inject] protected EmployeeController EmployeeController { get; set; } = default!;
        [Inject] protected IEmailService EmailService { get; set; } = default!;
        [Inject] protected IConfiguration Configuration { get; set; } = default!;
        [Inject] protected UserManager<ApplicationUser> UserManager { get; set; } = default!;
        [Inject] protected UserManagementService UserManagementService { get; set; } = default!;

        // --- State (alphabetically sorted, Id fields first) ---
        protected List<string> allRoles = new();
        protected string addUserConfirmPassword = string.Empty;
        protected string addUserEmail = string.Empty;
        protected string addUserPassword = string.Empty;
        protected string addUserSelectedRole = string.Empty;
        protected long addUserSelectedEmployeeId = 0;
        protected List<EmployeeViewModel> availableEmployees = new();
        protected string errorMessage = string.Empty;
        protected bool isLoading = true;
        protected bool isProcessing = false;
        protected ApplicationUser? pendingDeleteUser;
        protected bool showAddUserModal = false;
        protected bool showConfirmDeleteModal = false;
        protected bool showRoleModal = false;
        protected bool useDefaultPassword = true;
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
            addUserSelectedRole = string.Empty;
            addUserSelectedEmployeeId = 0;
            availableEmployees.Clear();
            useDefaultPassword = true;
            showAddUserModal = true;
        }

        private string GetDefaultPassword()
        {
            return Configuration["UserDefaults:DefaultPassword"] ?? string.Empty;
        }

        private async Task OnRoleChangedAsync(ChangeEventArgs e)
        {
            if (e.Value is string role)
            {
                addUserSelectedRole = role;
                addUserSelectedEmployeeId = 0;

                if (role == "employee")
                {
                    // Load available employees when employee role is selected
                    await LoadAvailableEmployeesAsync();
                }
                else
                {
                    availableEmployees.Clear();
                }

                StateHasChanged();
            }
        }

        private async Task LoadAvailableEmployeesAsync()
        {
            try
            {
                if (CurrentEnterpriseId.HasValue)
                {
                    // Get all employees for current enterprise
                    availableEmployees = await EmployeeController.Index(CurrentEnterpriseId.Value);
                    
                    // Filter out:
                    // 1. Employees without email addresses
                    // 2. Employees that already have user accounts
                    // 3. Employees whose initial password reset has been completed
                    var allUserEmails = users.Select(u => u.Email?.ToLower()).ToHashSet();
                    var completedPasswordResetEmails = users
                        .Where(u => u.InitialPasswordResetCompleted)
                        .Select(u => u.Email?.ToLower())
                        .ToHashSet();

                    availableEmployees = availableEmployees
                        .Where(e => !string.IsNullOrWhiteSpace(e.Email)
                            && !allUserEmails.Contains(e.Email?.ToLower())
                            && !completedPasswordResetEmails.Contains(e.Email?.ToLower()))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error loading employees: {ex.Message}";
                Console.WriteLine($"Error loading employees: {ex.Message}");
            }
        }

        private async Task CreateUserAsync()
        {
            errorMessage = string.Empty;

            var passwordToUse = useDefaultPassword ? GetDefaultPassword() : addUserPassword;

            if (string.IsNullOrWhiteSpace(passwordToUse))
            {
                errorMessage = "Default password is not configured. Please set UserDefaults:DefaultPassword in appsettings.json.";
                return;
            }

            // Validate common fields
            if (!useDefaultPassword)
            {
                if (string.IsNullOrWhiteSpace(addUserPassword) || string.IsNullOrWhiteSpace(addUserConfirmPassword))
                {
                    errorMessage = "Password is required.";
                    return;
                }

                if (addUserPassword != addUserConfirmPassword)
                {
                    errorMessage = "Passwords do not match. Please try again.";
                    return;
                }
            }

            if (passwordToUse.Length < 6)
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
                ApplicationUser newUser;

                if (addUserSelectedRole == "employee")
                {
                    // Create user from selected employee
                    if (addUserSelectedEmployeeId == 0)
                    {
                        errorMessage = "Please select an employee.";
                        isProcessing = false;
                        return;
                    }

                    var selectedEmployee = availableEmployees.FirstOrDefault(e => e.EmployeeID == addUserSelectedEmployeeId);
                    if (selectedEmployee == null || string.IsNullOrWhiteSpace(selectedEmployee.Email))
                    {
                        errorMessage = "Selected employee does not have an email address.";
                        isProcessing = false;
                        return;
                    }

                    newUser = new ApplicationUser
                    {
                        UserName = selectedEmployee.Email,
                        Email = selectedEmployee.Email,
                        EntrepriseId = CurrentEnterpriseId.Value,
                        InitialPasswordResetCompleted = false
                    };
                }
                else
                {
                    // Create regular user with manual email
                    if (string.IsNullOrWhiteSpace(addUserEmail))
                    {
                        errorMessage = "Email is required.";
                        isProcessing = false;
                        return;
                    }

                    var existingUser = await UserManagementService.FindUserByEmailAsync(addUserEmail);
                    if (existingUser != null)
                    {
                        errorMessage = "User with this email already exists.";
                        isProcessing = false;
                        return;
                    }

                    newUser = new ApplicationUser
                    {
                        UserName = addUserEmail,
                        Email = addUserEmail,
                        EntrepriseId = CurrentEnterpriseId.Value,
                        InitialPasswordResetCompleted = false
                    };
                }

                var result = await UserManagementService.CreateUserAsync(newUser, passwordToUse);
                if (result.Succeeded)
                {
                    // Assign the selected role
                    if (!string.IsNullOrEmpty(addUserSelectedRole))
                    {
                        await UserManagementService.AddRoleAsync(newUser, addUserSelectedRole);
                    }

                    // Send password reset email
                    try
                    {
                        var resetToken = await UserManager.GeneratePasswordResetTokenAsync(newUser);
                        var baseUrl = Navigation.BaseUri.TrimEnd('/');
                        var resetUrl = $"{baseUrl}/reset-password";
                        
                        await EmailService.SendPasswordResetEmailAsync(newUser.Email!, newUser.Email ?? newUser.UserName ?? "User", resetToken, resetUrl);
                    }
                    catch (Exception emailEx)
                    {
                        Console.WriteLine($"Error sending password reset email: {emailEx.Message}");
                        // Don't fail user creation if email fails
                    }

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
            addUserSelectedRole = string.Empty;
            addUserSelectedEmployeeId = 0;
            availableEmployees.Clear();
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
