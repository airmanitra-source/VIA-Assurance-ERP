using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class AddFleet : AuthenticatedComponentBase
    {
        // --- Parameters ---
        [Parameter][SupplyParameterFromQuery] public long? Id { get; set; }

        // --- Injections ---
        [Inject] protected FleetController FleetController { get; set; } = default!;

        // --- State (alphabetically sorted, ID first if any) ---
        protected EntrepriseViewModel? currentCompany;
        protected List<string> errors = new();
        protected EntrepriseFleetViewModel fleetModel = new();
        protected bool isLoadingCompany = true;
        protected bool isLoadingItem = false;
        protected bool isSubmitting = false;
        protected string successMessage = string.Empty;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            var company = await GetOrLoadCurrentCompanyAsync();
            currentCompany = EntrepriseViewModel.FromBusinessModel(company);
            isLoadingCompany = false;

            if (Id.HasValue && currentCompany != null)
            {
                await LoadFleetItem(Id.Value);
            }
        }

        #region "Private"
        private async Task LoadFleetItem(long id)
        {
            isLoadingItem = true;
            try
            {
                if (currentCompany == null) return;
                var item = await FleetController.Show(id, currentCompany.Id);
                if (item != null)
                {
                    fleetModel = item;
                }
                else
                {
                    errors.Add("Fleet item not found.");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error loading fleet data: {ex.Message}");
            }
            finally
            {
                isLoadingItem = false;
            }
        }

        private async Task HandleSubmit()
        {
            if (currentCompany == null) return;

            isSubmitting = true;
            errors.Clear();
            successMessage = string.Empty;

            try
            {
                var result = await FleetController.Store(fleetModel, currentCompany.Id, currentCompany.RaisonSocial);
                if (result.Success)
                {
                    successMessage = result.Message;
                    if (!Id.HasValue)
                    {
                        fleetModel = new EntrepriseFleetViewModel();
                    }
                    else
                    {
                        // Refresh data to show isInsured status or insurance policy number
                        await LoadFleetItem(Id.Value);
                    }
                }
                else
                {
                    errors.AddRange(result.Errors);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"An error occurred while saving: {ex.Message}");
            }
            finally
            {
                isSubmitting = false;
            }
        }
        #endregion
    }
}