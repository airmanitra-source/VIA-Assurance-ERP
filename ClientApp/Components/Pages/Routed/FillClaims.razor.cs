using ClientApp.Models;
using ClientApp.Services;
using Company.Fleet.Module;
using Company.Module;
using Company.Transportation.Module;
using Company.Warehouse.Module;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class FillClaims: ComponentBase
    {
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] AuthenticationService AuthService { get; set; } = default!;
        [Inject] protected ICompanyFleetModule FleetModule { get; set; } = default!;
        [Inject] protected ICompanyTransportationModule TransportationModule { get; set; } = default!;
        [Inject] protected ICompanyWarehouseModule WarehouseModule { get; set; } = default!;
        [Inject] protected ICompanyModule CompanyModule { get; set; } = default!;

        protected bool isLoadingCompany = true;
        protected bool isLoadingAssets = true;
        protected bool isSubmitting = false;
        protected Company.Module.Business.EntrepriseBusinessModel? currentCompany;
        protected string selectedAssetType = string.Empty;
        protected long selectedAssetId = 0;
        protected string successMessage = string.Empty;
        protected string errorMessage = string.Empty;

        protected CompanySinisterViewModel sinisterModel = new();
        protected List<FleetViewModel> fleets = new();
        protected List<TransportationViewModel> transportations = new();
        protected List<WarehouseViewModel> warehouses = new();

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

                    // Load fleets and convert to ViewModels
                    var fleetList = await FleetModule.GetCompanyFleetAsync(currentCompany.Id);
                    fleets = fleetList.Select(f => new FleetViewModel
                    {
                        Id = f.Id,
                        Type = f.Type,
                        Year = f.Year,
                        IsWorking = f.IsWorking,
                        Mileage = f.Mileage,
                        Make = f.Make,
                        Model = f.Model,
                        WantsInsurance = f.WantsInsurance,
                        IsInsured = f.IsInsured
                    }).ToList();

                    // Load transportations and convert to ViewModels
                    var transportationList = await TransportationModule.GetCompanyTransportationsAsync(currentCompany.Id);
                    transportations = transportationList.Select(t => new TransportationViewModel
                    {
                        Id = t.Id,
                        Description = t.Description,
                        Value = t.Value,
                        DepartureDate = t.DepartureDate,
                        ArrivalDate = t.ArrivalDate,
                        Origin = t.Origin,
                        Destination = t.Destination,
                        Frequency = t.Frequency ?? "OneTime",
                        WantsInsurance = t.WantsInsurance,
                        IsInsured = t.IsInsured
                    }).ToList();

                    // Load warehouses and convert to ViewModels
                    var warehouseList = await WarehouseModule.GetCompanyWarehousesAsync(currentCompany.Id);
                    warehouses = warehouseList.Select(w => new WarehouseViewModel
                    {
                        Id = w.Id,
                        Name = w.Name,
                        SizeM2 = w.SizeM2,
                        ContentsDescription = w.ContentsDescription,
                        Address = w.Address,
                        WantsInsurance = w.WantsInsurance,
                        IsInsured = w.IsInsured
                    }).ToList();

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

                isSubmitting = true;

                // Assign asset ID based on type
                sinisterModel.EntrepriseId = currentCompany?.Id ?? 0;
                sinisterModel.AssetType = selectedAssetType;

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

                // TODO: Call the sinister module to save the claim
                // For now, simulate submission
                await Task.Delay(500);

                successMessage = Localizer["ClaimSubmittedSuccessfully"];

                // Reset form
                sinisterModel = new();
                selectedAssetType = string.Empty;
                selectedAssetId = 0;

                // Redirect after 2 seconds
                await Task.Delay(2000);
                Navigation.NavigateTo("");
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