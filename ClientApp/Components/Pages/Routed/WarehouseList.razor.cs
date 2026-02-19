using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.Localization;
using ClientApp.Models;
using ClientApp.Services;
using Company.Warehouse.Module;
using Company.Warehouse.Module.Business;
using Company.Module;

namespace ClientApp.Components.Pages.Routed
{
    public partial class WarehouseList : ComponentBase
    {
        private List<EntrepriseWarehouseBusinessModel>? warehouses;
        private bool isLoading = true;

        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationService AuthService { get; set; } = default!;
        [Inject] private ICompanyWarehouseModule WarehouseModule { get; set; } = default!;
        [Inject] private ICompanyModule CompanyModule { get; set; } = default!;
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
                    var company = await CompanyModule.GetCompanyByIdAsync(entrepriseId.Value);
                    if (company != null)
                    {
                        var result = await WarehouseModule.GetCompanyWarehousesAsync(company.Id);
                        warehouses = result.ToList();
                    }
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

        private void EditWarehouse(EntrepriseWarehouseBusinessModel wh)
        {
            Navigation.NavigateTo($"/add-warehouse?id={wh.Id}");
        }

        private async Task DeleteWarehouse(EntrepriseWarehouseBusinessModel wh)
        {
            if (await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {wh.Name}?"))
            {
                await WarehouseModule.RemoveWarehouseAsync(wh.Id);
                await LoadData();
            }
        }
    }
}
