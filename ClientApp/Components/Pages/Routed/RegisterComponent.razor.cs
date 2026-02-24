using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using ClientApp.Models;
using ClientApp.Services;

namespace ClientApp.Components.Pages.Routed
{
    public partial class RegisterComponent : ComponentBase
    {
        [SupplyParameterFromForm]
        private RegisterViewModel registerModel { get; set; } = new();
        private List<string> errors = new();
        private bool isLoading = false;

        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationService AuthService { get; set; } = default!;
        [Inject] private FileTable.Infrastructure.Identities.ApplicationUserStore UserStore { get; set; } = default!;

        private async Task HandleRegister()
        {
            isLoading = true;
            errors.Clear();
            bool shouldNavigate = false;

            try
            {
                // 1. Create the Entreprise first
                var entrepriseId = await UserStore.CreateEntrepriseAsync(registerModel.RaisonSocial, registerModel.Email);

                // 2. Register the user linked to that Entreprise
                var result = await AuthService.RegisterAsync(entrepriseId, registerModel.Email, registerModel.Password);
                if (result.Succeeded)
                {
                    shouldNavigate = true;
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        errors.Add(error.Description);
                    }
                }
            }
            catch (NavigationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                errors.Add($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }

            if (shouldNavigate)
            {
                Navigation.NavigateTo("/documents", forceLoad: true);
            }
        }
    }
}
