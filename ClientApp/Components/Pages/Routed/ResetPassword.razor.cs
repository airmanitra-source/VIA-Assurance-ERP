using ClientApp.Controllers;
using FileTable.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class ResetPassword : ComponentBase
    {
        // --- Parameters ---
        [Parameter][SupplyParameterFromQuery] public string? Token { get; set; }
        [Parameter][SupplyParameterFromQuery] public string? Email { get; set; }

        // --- Injections ---
        [Inject] protected ResetPasswordController Controller { get; set; } = default!;
        [Inject] protected IEmailService EmailService { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;

        // --- State (alphabetically sorted) ---
        protected string confirmPassword = string.Empty;
        protected string errorMessage = string.Empty;
        protected bool isInvalidToken = false;
        protected bool isLoading = true;
        protected bool isProcessing = false;
        protected string newPassword = string.Empty;
        protected bool passwordResetSuccess = false;
        protected string successMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            // Note: Blazor automatically URL-decodes query parameters via [SupplyParameterFromQuery]
            // No manual decoding needed!

            // Verify token is valid
            if (string.IsNullOrWhiteSpace(Token) || string.IsNullOrWhiteSpace(Email))
            {
                isInvalidToken = true;
                errorMessage = "Invalid password reset link. Missing token or email.";
            }
            else
            {
                var isValid = await Controller.VerifyTokenAsync(Email, Token);
                if (!isValid)
                {
                    isInvalidToken = true;
                    errorMessage = "Invalid or expired password reset link.";
                }
            }

            isLoading = false;
        }

        #region Event Handlers

        protected async Task HandleResetAsync()
        {
            errorMessage = string.Empty;
            successMessage = string.Empty;

            // Validation
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                errorMessage = "Password is required.";
                return;
            }

            if (newPassword != confirmPassword)
            {
                errorMessage = "Passwords do not match.";
                return;
            }

            if (newPassword.Length < 6)
            {
                errorMessage = "Password must be at least 6 characters long.";
                return;
            }

            isProcessing = true;
            try
            {
                var (success, message) = await Controller.ResetPasswordAsync(Email!, Token!, newPassword, EmailService);
                
                if (success)
                {
                    passwordResetSuccess = true;
                    successMessage = message;
                    newPassword = string.Empty;
                    confirmPassword = string.Empty;

                    // Redirect to login after 3 seconds
                    await Task.Delay(3000);
                    Navigation.NavigateTo("/login");
                }
                else
                {
                    errorMessage = message;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"An error occurred: {ex.Message}";
                Console.WriteLine($"Error resetting password: {ex.Message}");
            }
            finally
            {
                isProcessing = false;
            }
        }

        protected void NavigateToLogin()
        {
            Navigation.NavigateTo("/login");
        }

        #endregion
    }
}
