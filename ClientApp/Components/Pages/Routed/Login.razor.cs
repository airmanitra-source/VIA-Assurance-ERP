using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ClientApp.Models;
using ClientApp.Services;

namespace ClientApp.Components.Pages.Routed
{
    public partial class Login : ComponentBase
    {
        [SupplyParameterFromForm]
        private LoginViewModel loginModel { get; set; } = new();
        private string errorMessage = string.Empty;
        private bool isLoading = false;

        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationService AuthService { get; set; } = default!;

        private async Task HandleLogin()
        {
            isLoading = true;
            errorMessage = string.Empty;
            bool shouldNavigate = false;

            try
            {
                var result = await AuthService.LoginAsync(loginModel.Username, loginModel.Password);
                if (result)
                {
                    shouldNavigate = true;
                }
                else
                {
                    errorMessage = "Invalid username or password. Please try again.";
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
                Navigation.NavigateTo("/list-employees");
            }
        }
    }
}
