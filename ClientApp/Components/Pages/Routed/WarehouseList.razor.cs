using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.Localization;
using ClientApp.Models;
using ClientApp.Services;
using ClientApp.Controllers;

namespace ClientApp.Components.Pages.Routed
{
    public partial class WarehouseList : ComponentBase
    {
        private List<WarehouseViewModel>? warehouses;
        private bool isLoading = true;

        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationService AuthService { get; set; } = default!;
        [Inject] private WarehouseController WarehouseController { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (!AuthService.IsAuthenticated())
            {
                Navigation.NavigateTo("/login");
                return;
            }

            await LoadData();
        }

        private async Task LoadData()
        {
            isLoading = true;
            try
            {
                var entrepriseId = AuthService.GetCurrentEntrepriseId();
                if (entrepriseId.HasValue)
                {
                    warehouses = await WarehouseController.Index(entrepriseId.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading warehouses: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void EditWarehouse(WarehouseViewModel wh)
        {
            Navigation.NavigateTo($"/add-warehouse?id={wh.Id}");
        }

        private async Task DeleteWarehouse(WarehouseViewModel wh)
        {
            if (await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {wh.Name}?"))
            {
                await WarehouseController.Destroy(wh.Id);
                await LoadData();
            }
        }
    }
}
