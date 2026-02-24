namespace ClientApp.Components.Layout
{
    public partial class NavMenu
    {
        private bool showEmployeesMenu = false;
        private bool showInsurancesMenu = false;

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