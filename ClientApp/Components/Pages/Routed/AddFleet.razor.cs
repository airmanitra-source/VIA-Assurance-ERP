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

namespace ClientApp.Components.Pages.Routed
{
    public partial class AddFleet : AuthenticatedComponentBase
    {
        [Inject] protected ICompanyDocumentModule DocumentModule { get; set; } = default!;
        [Inject] protected ICompanyFleetModule FleetModule { get; set; } = default!;

        [Parameter][SupplyParameterFromQuery] public long? Id { get; set; }

        protected FleetViewModel fleetModel = new();
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
                var item = await FleetModule.GetFleetItemAsync(id);
                if (item != null)
                {
                    fleetModel = new FleetViewModel
                    {
                        Type = item.Type,
                        Year = item.Year,
                        IsWorking = item.IsWorking,
                        Mileage = item.Mileage,
                        Make = item.Make,
                        Model = item.Model,
                        WantsInsurance = item.WantsInsurance,
                        IsInsured = item.IsInsured,
                        PolicyNumber = item.PolicyNumber
                    };
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
                // Reset WantsInsurance if no longer eligible
                if (!fleetModel.IsEligibleForInsurance)
                {
                    fleetModel.WantsInsurance = false;
                    fleetModel.IsInsured = false;
                }

                var fleetBusinessModel = new EntrepriseFleetBusinessModel
                {
                    Id = Id ?? 0,
                    EntrepriseId = currentCompany.Id,
                    Type = fleetModel.Type,
                    Year = fleetModel.Year,
                    IsWorking = fleetModel.IsWorking,
                    Mileage = fleetModel.Mileage,
                    Make = fleetModel.Make,
                    Model = fleetModel.Model,
                    WantsInsurance = fleetModel.WantsInsurance,
                    IsInsured = fleetModel.IsInsured
                };

                long fleetId = Id ?? 0;
                if (Id.HasValue)
                {
                    await FleetModule.SetFleetItemAsync(fleetBusinessModel);
                    successMessage = "Vehicle updated successfully!";
                }
                else
                {
                    fleetId = await FleetModule.AddFleetItemAsync(fleetBusinessModel);
                    successMessage = "Vehicle added successfully!";
                }

                if (fleetModel.WantsInsurance && !fleetModel.IsInsured)
                {
                    fleetModel.PolicyNumber = "FLT-" + fleetId + "-" + DateTime.Now.Ticks.ToString().Substring(12);

                    var policyData = new PolicyPdfModel
                    {
                        PolicyNumber = fleetModel.PolicyNumber,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddYears(1),
                        InsuredName = currentCompany.RaisonSocial,
                        Address = "N/A",
                        VehicleDescription = $"{fleetModel.Year} {fleetModel.Make} {fleetModel.Model} ({fleetModel.Type})",
                        VIN = "N/A",
                        VehicleCoverages = new List<CoverageModel>
                        {
                            new CoverageModel { Description = "Chapitre A - Responsabilité civile", Amount = 2000000 },
                            new CoverageModel { Description = "Chapitre B3 - Tous les risques sauf collision ou renversement", Deductible = 500 }
                        },
                        PolicyCoverages = new List<CoverageModel>
                        {
                            new CoverageModel { Description = "F.A.Q. no 27 Resp. civile domm. véh. n'appartenant pas à l'assuré", Amount = 100000 }
                        }
                    };

                    await DocumentModule.GenerateAndLinkPolicyConfirmationAsync(currentCompany.Id, "Fleet", policyData, fleetId);

                    // Mark as insured in database
                    fleetBusinessModel.Id = fleetId;
                    fleetBusinessModel.IsInsured = true;
                    fleetBusinessModel.PolicyNumber = fleetModel.PolicyNumber;
                    await FleetModule.SetFleetItemAsync(fleetBusinessModel);
                    
                    fleetModel.IsInsured = true;
                }

                if (!Id.HasValue)
                {
                    fleetModel = new FleetViewModel();
                }

                _ = Task.Delay(3000).ContinueWith(_ => {
                    InvokeAsync(() => {
                        successMessage = string.Empty;
                        StateHasChanged();
                    });
                });
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to save: {ex.Message}");
            }
            finally
            {
                isSubmitting = false;
            }
        }
        #endregion
    }
}