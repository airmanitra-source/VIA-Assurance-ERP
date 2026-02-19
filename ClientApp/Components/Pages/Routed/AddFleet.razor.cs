using ClientApp.Components.Shared;
using ClientApp.Models;
using Company.Fleet.Module.Business;
using Company.Module.Business;
using Microsoft.AspNetCore.Components;
using CompanyDocuments.Module.Business;
using Company.Fleet.Module;
using Company.Module;
using CompanyDocuments.Module;
using ClientApp.Services;
using ClientApp.Controllers;

namespace ClientApp.Components.Pages.Routed
{
    public partial class AddFleet : AuthenticatedComponentBase
    {
        [Inject] protected ICompanyDocumentModule DocumentModule { get; set; } = default!;
        [Inject] protected FleetController FleetController { get; set; } = default!;

        [Parameter][SupplyParameterFromQuery] public long? Id { get; set; }

        protected EntrepriseFleetViewModel fleetModel = new();
        protected EntrepriseBusinessModel? currentCompany;
        protected bool isLoadingCompany = true;
        protected bool isLoadingItem = false;
        protected bool isSubmitting = false;
        protected List<string> errors = new();
        protected string successMessage = string.Empty;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadCurrentCompany();

            if (Id.HasValue && currentCompany != null)
            {
                await LoadFleetItem(Id.Value);
            }
        }

        #region "Private"
        private async Task LoadCurrentCompany()
        {
            isLoadingCompany = true;
            try
            {
                currentCompany = await GetOrLoadCurrentCompanyAsync();
            }
            catch (Exception ex)
            {
                errors.Add($"Error loading company data: {ex.Message}");
            }
            finally
            {
                isLoadingCompany = false;
            }
        }

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
                var result = await FleetController.Store(fleetModel, currentCompany.Id);
                if (result.Success)
                {
                    successMessage = result.Message;
                    if (!Id.HasValue)
                    {
                        fleetModel = new EntrepriseFleetViewModel();
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