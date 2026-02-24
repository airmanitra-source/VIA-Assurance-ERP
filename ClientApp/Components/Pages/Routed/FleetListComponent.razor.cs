using ClientApp.Components.Shared;
using ClientApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ClientApp.Controllers;

namespace ClientApp.Components.Pages.Routed
{
    public partial class FleetListComponent : AuthenticatedComponentBase
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
            // Protection: ne pas permettre la suppression des véhicules assurés
            if (item.IsInsured)
            {
                await JSRuntime.InvokeVoidAsync("alert", 
                    "❌ Ce véhicule est assuré et ne peut pas être supprimé.\n\n" +
                    "La confirmation d'assurance a été signée et le véhicule est protégé contre la suppression.");
                return;
            }

            var confirmMessage = $"Êtes-vous sûr de vouloir supprimer ce véhicule?\n\n" +
                                $"🚗 {item.Make} {item.Model} ({item.Year})\n";
            
            if (!string.IsNullOrEmpty(item.LicensePlate))
            {
                confirmMessage += $"📋 Plaque: {item.LicensePlate}\n";
            }
            
            if (!string.IsNullOrEmpty(item.VIN))
            {
                confirmMessage += $"🔢 VIN: {item.VIN}\n";
            }

            if (await JSRuntime.InvokeAsync<bool>("confirm", confirmMessage))
            {
                try
                {
                    await FleetController.Destroy(item.Id);
                    await LoadDataAsync();
                    await JSRuntime.InvokeVoidAsync("alert", "✅ Véhicule supprimé avec succès!");
                }
                catch (InvalidOperationException ex)
                {
                    // Exception de protection du module
                    await JSRuntime.InvokeVoidAsync("alert", $"❌ {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Autres erreurs
                    await JSRuntime.InvokeVoidAsync("alert", $"❌ Erreur: {ex.Message}");
                    Console.WriteLine($"Error deleting fleet item: {ex}");
                }
            }
        }
        #endregion

    }
}