using ClientApp.Models;
using ClientApp.Services;
using Company.Module;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ClientApp.Controllers;

namespace ClientApp.Components.Pages.Routed
{
    public partial class FillClaimsComponent : ComponentBase
    {
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] AuthenticationService AuthService { get; set; } = default!;
        [Inject] protected ClaimController ClaimController { get; set; } = default!;
        [Inject] protected ICompanyModule CompanyModule { get; set; } = default!;

        protected bool isLoadingCompany = true;
        protected bool isLoadingAssets = true;
        protected bool isSubmitting = false;
        protected List<CompanySinisterAttachedDocumentViewModel> attachedDocuments = new();
        protected Company.Module.Business.EntrepriseBusinessModel? currentCompany;
        protected string selectedAssetType = string.Empty;
        protected long selectedAssetId = 0;
        protected string successMessage = string.Empty;
        protected string errorMessage = string.Empty;
        protected List<long> selectedSinisterTypeIds = new();

        protected CompanySinisterViewModel sinisterModel = new();
        protected List<EntrepriseFleetViewModel> fleets = new();
        protected List<TransportationViewModel> transportations = new();
        protected List<WarehouseViewModel> warehouses = new();
        protected List<SinisterTypeViewModel> availableSinisterTypes = new();

        protected override async Task OnInitializedAsync()
        {
            if (!AuthService.IsAuthenticated())
            {
                Navigation.NavigateTo("/login");
                return;
            }

            try
            {
                isLoadingCompany = true;
                var entrepriseId = AuthService.GetCurrentEntrepriseId();
                if (entrepriseId.HasValue)
                {
                    currentCompany = await CompanyModule.GetCompanyByIdAsync(entrepriseId.Value);
                }

                if (currentCompany != null)
                {
                    isLoadingAssets = true;

                    var assets = await ClaimController.Index(currentCompany.Id);
                    fleets = assets.Fleets;
                    transportations = assets.Transportations;
                    warehouses = assets.Warehouses;
                    availableSinisterTypes = assets.SinisterTypes;

                    isLoadingAssets = false;
                }
            }
            finally
            {
                isLoadingCompany = false;
            }
        }

        private void OnAssetTypeChanged(ChangeEventArgs e)
        {
            selectedAssetType = e.Value?.ToString() ?? string.Empty;
            selectedAssetId = 0;
            sinisterModel.AssetType = selectedAssetType;
            sinisterModel.EntrepriseFleetId = null;
            sinisterModel.EntrepriseMerchandiseTransportationId = null;
            sinisterModel.EntrepriseWarehouseId = null;
        }

        protected void OnPhotoSelected(InputFileChangeEventArgs e)
        {
            attachedDocuments.Clear();
            var file = e.File;
            if (file == null)
            {
                return;
            }

            attachedDocuments.Add(new CompanySinisterAttachedDocumentViewModel
            {
                File = file,
                TypeDocument = "Photo"
            });
        }

        protected void RemoveDocument(CompanySinisterAttachedDocumentViewModel doc)
        {
            attachedDocuments.Remove(doc);
        }

        protected void ToggleSinisterType(long typeId)
        {
            if (selectedSinisterTypeIds.Contains(typeId))
            {
                selectedSinisterTypeIds.Remove(typeId);
            }
            else
            {
                selectedSinisterTypeIds.Add(typeId);
            }
        }

        protected bool IsSinisterTypeSelected(long typeId)
        {
            return selectedSinisterTypeIds.Contains(typeId);
        }

        private async Task HandleSubmit()
        {
            try
            {
                successMessage = string.Empty;
                errorMessage = string.Empty;

                // Validate selection
                if (string.IsNullOrEmpty(selectedAssetType) || selectedAssetId == 0)
                {
                    errorMessage = Localizer["PleaseSelectAsset"];
                    return;
                }

                if (!selectedSinisterTypeIds.Any())
                {
                    errorMessage = Localizer["SinisterTypeRequired"];
                    return;
                }

                isSubmitting = true;

                // Assign asset ID based on type
                sinisterModel.EntrepriseId = currentCompany?.Id ?? 0;
                sinisterModel.AssetType = selectedAssetType;
                sinisterModel.ResolvedAmount = null;
                sinisterModel.Status = "Pending";
                sinisterModel.SelectedSinisterTypeIds = selectedSinisterTypeIds;

                switch (selectedAssetType)
                {
                    case "Fleet":
                        sinisterModel.EntrepriseFleetId = selectedAssetId;
                        sinisterModel.EntrepriseMerchandiseTransportationId = null;
                        sinisterModel.EntrepriseWarehouseId = null;
                        break;
                    case "Transportation":
                        sinisterModel.EntrepriseFleetId = null;
                        sinisterModel.EntrepriseMerchandiseTransportationId = selectedAssetId;
                        sinisterModel.EntrepriseWarehouseId = null;
                        break;
                    case "Warehouse":
                        sinisterModel.EntrepriseFleetId = null;
                        sinisterModel.EntrepriseMerchandiseTransportationId = null;
                        sinisterModel.EntrepriseWarehouseId = selectedAssetId;
                        break;
                }

                var result = await ClaimController.Store(sinisterModel, currentCompany?.Id ?? 0, attachedDocuments);
                if (result.Success)
                {
                    successMessage = result.Message;

                    // Reset form
                    sinisterModel = new();
                    attachedDocuments.Clear();
                    selectedAssetType = string.Empty;
                    selectedAssetId = 0;
                    selectedSinisterTypeIds.Clear();

                    // Redirect after 2 seconds
                    await Task.Delay(2000);
                    Navigation.NavigateTo("/list-sinisters");
                }
                else
                {
                    errorMessage = result.Errors.Any() ? string.Join(" ", result.Errors) : result.Message;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"{Localizer["ErrorSubmittingClaim"]}: {ex.Message}";
            }
            finally
            {
                isSubmitting = false;
            }
        }
    }
}