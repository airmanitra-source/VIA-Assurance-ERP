using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq;
using System.Security.Claims;

namespace ClientApp.Components.Layout
{
    public partial class NavMenu
    {
        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        private bool IsRHOnly { get; set; }
        private bool showEmployeesMenu = false;
        private bool showInsurancesMenu = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            
            // "RH seulement" might include 'employee' as it's a base role, 
            // but we check if any other management roles are present.
            var managementRoles = new[] { "directeur", "auditeur", "admin", "developer" };
            var isRH = roles.Contains("RH");
            var hasOtherManagementRoles = roles.Any(r => managementRoles.Contains(r));

            IsRHOnly = isRH && !hasOtherManagementRoles;
        }

        private void ToggleEmployeesMenu()
        {
            showEmployeesMenu = !showEmployeesMenu;
            if (showEmployeesMenu) showInsurancesMenu = false;
        }

        private void ToggleInsurancesMenu()
        {
            showInsurancesMenu = !showInsurancesMenu;
            if (showInsurancesMenu) showEmployeesMenu = false;
        }

        private void FillClaims()
        {
            Navigation.NavigateTo($"/fill-claims");
        }
    }
}