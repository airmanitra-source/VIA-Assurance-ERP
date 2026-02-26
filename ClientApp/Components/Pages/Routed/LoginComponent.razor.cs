using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ClientApp.Models;
using ClientApp.Services;
using System.Security.Claims;

namespace ClientApp.Components.Pages.Routed
{
    public partial class LoginComponent : ComponentBase
    {
        [SupplyParameterFromForm]
        private LoginViewModel loginModel { get; set; } = new();
        private string errorMessage = string.Empty;
        private bool isLoading = false;

        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationService AuthService { get; set; } = default!;
        [Inject] private CustomAuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        private async Task HandleLogin()
        {
            isLoading = true;
            errorMessage = string.Empty;
            bool shouldNavigate = false;

            try
            {
                var (success, message) = await AuthService.LoginAsync(loginModel.Username, loginModel.Password);
                if (success)
                {
                    shouldNavigate = true;
                }
                else
                {
                    errorMessage = message ?? "Invalid username or password. Please try again.";
                }
            }
            catch (NavigationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                errorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                isLoading = false;
            }

            if (shouldNavigate)
            {
                // Get the user's roles to determine the redirect URL
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                
                // Check if user has the "employee" role
                if (user.IsInRole("employee"))
                {
                    Navigation.NavigateTo("/employee-dashboard");
                }
                else
                {
                    Navigation.NavigateTo("/list-employees");
                }
            }
        }
    }
}
