using ClientApp.Services;
using Company.Module;
using Company.Module.Business;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Shared
{
    public abstract class AuthenticatedComponentBase : ComponentBase
    {
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected AuthenticationService AuthService { get; set; } = default!;
        [Inject] protected ICompanyModule CompanyModule { get; set; } = default!;

        protected long? CurrentEnterpriseId => AuthService.GetCurrentEntrepriseId();
        protected string? CurrentUserEmail => AuthService.GetCurrentUserEmail();

        protected async Task HandleLogoutAsync()
        {
            await AuthService.LogoutAsync();
            Navigation.NavigateTo("/login", forceLoad: true);
        }

        protected override async Task OnInitializedAsync()
        {
            if (!AuthService.IsAuthenticated())
            {
                Navigation.NavigateTo("/login");
                return;
            }

            await OnInitializedAuthenticatedAsync();
        }

        /// <summary>
        /// Hook for child components to perform initialization logic after authentication is verified.
        /// </summary>
        protected virtual Task OnInitializedAuthenticatedAsync() => Task.CompletedTask;

        /// <summary>
        /// Helper to get the current company details if needed.
        /// </summary>
        protected async Task<EntrepriseBusinessModel?> GetOrLoadCurrentCompanyAsync()
        {
            var id = CurrentEnterpriseId;
            if (id.HasValue)
            {
                return await CompanyModule.GetCompanyByIdAsync(id.Value);
            }
            return null;
        }
    }
}
