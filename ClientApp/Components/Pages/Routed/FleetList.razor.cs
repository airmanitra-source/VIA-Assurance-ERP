using ClientApp.Components.Shared;
using ClientApp.Models;
using Company.Fleet.Module;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ClientApp.Controllers;

namespace ClientApp.Components.Pages.Routed
{
    public partial class FleetList: AuthenticatedComponentBase
    {
        [Inject]
        protected FleetController FleetController { get; set; } = default!;

        [Inject] 
        protected IJSRuntime JSRuntime { get; set; } = default!;

        protected List<EntrepriseFleetViewModel>? fleetItems;
        
        protected bool isLoading = true;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadDataAsync();
        }

        #region Private
        private async Task LoadDataAsync()
        {
            isLoading = true;
            try
            {
                var company = await GetOrLoadCurrentCompanyAsync();
                if (company != null)
                {
                    fleetItems = await FleetController.Index(company.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading fleet: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void EditItem(EntrepriseFleetViewModel item)
        {
            Navigation.NavigateTo($"/add-fleet?id={item.Id}");
        }

        private async Task DeleteItemAsync(EntrepriseFleetViewModel item)
        {
            if (await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {item.Make} {item.Model}?"))
            {
                await FleetController.Destroy(item.Id);
                await LoadDataAsync();
            }
        }
        #endregion

    }
}