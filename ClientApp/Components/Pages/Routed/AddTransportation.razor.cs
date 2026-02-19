using ClientApp.Models;
using ClientApp.Services;
using Company.Module;
using Company.Transportation.Module;
using Company.Transportation.Module.Data.Models;
using Company.Transportation.Module.Business;
using CompanyDocuments.Module.Business;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class AddTransportation: ComponentBase
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public long? Id { get; set; }

        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;

        [Inject]
        protected AuthenticationService AuthService { get; set; } = default!;

        [Inject]
        protected ICompanyTransportationModule TransportationModule { get; set; } = default!;

        [Inject]
        protected ICompanyModule CompanyModule { get; set; } = default!;

        [Inject]
        protected global::CompanyDocuments.Module.ICompanyDocumentModule DocumentModule { get; set; } = default!;

        protected TransportationViewModel model = new();
        protected EntrepriseViewModel? currentCompany;
        protected bool isLoadingCompany = true;
        protected bool isLoadingItem = false;
        protected bool isSubmitting = false;
        protected List<string> errors = new();
        protected string successMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (!AuthService.IsAuthenticated())
            {
                Navigation.NavigateTo("/login");
                return;
            }

            await LoadCurrentCompany();

            if (Id.HasValue && currentCompany != null)
            {
                await LoadItem(Id.Value);
            }
        }

        #region Private
        private async Task LoadCurrentCompany()
        {
            isLoadingCompany = true;
            try
            {
                var entrepriseId = AuthService.GetCurrentEntrepriseId();
                if (entrepriseId.HasValue)
                {
                    var currentCompanyBusinessModel = await CompanyModule.GetCompanyByIdAsync(entrepriseId.Value);
                    currentCompany = EntrepriseViewModel.FromBusinessModel(currentCompanyBusinessModel);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error: {ex.Message}");
            }
            finally
            {
                isLoadingCompany = false;
            }
        }

        private async Task LoadItem(long id)
        {
            isLoadingItem = true;
            try
            {
                var item = await TransportationModule.GetTransportationAsync(id);
                if (item != null)
                {
                    model = new TransportationViewModel
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Value = item.Value,
                        DepartureDate = item.DepartureDate,
                        ArrivalDate = item.ArrivalDate,
                        Origin = item.Origin,
                        Destination = item.Destination,
                        Frequency = item.Frequency,
                        WantsInsurance = item.WantsInsurance,
                        IsInsured = item.IsInsured,
                        PolicyNumber = item.PolicyNumber
                    };
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error: {ex.Message}");
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
                var businessModel = new EntrepriseMerchandiseTransportationBusinessModel
                {
                    Id = Id ?? 0,
                    EntrepriseId = currentCompany.Id,
                    Description = model.Description,
                    Value = model.Value,
                    DepartureDate = model.DepartureDate,
                    ArrivalDate = model.ArrivalDate,
                    Origin = model.Origin,
                    Destination = model.Destination,
                    Frequency = model.Frequency,
                    WantsInsurance = model.WantsInsurance,
                    IsInsured = model.IsInsured,
                    PolicyNumber = model.PolicyNumber
                };

                long transId = Id ?? 0;
                if (Id.HasValue)
                {
                    await TransportationModule.SetTransportationAsync(businessModel);
                    successMessage = "Transportation updated successfully!";
                }
                else
                {
                    transId = await TransportationModule.AddTransportationAsync(businessModel);
                    successMessage = "Transportation added successfully!";
                }

                if (model.WantsInsurance && !model.IsInsured)
                {
                    model.PolicyNumber = "TRN-" + transId + "-" + DateTime.Now.Ticks.ToString().Substring(12);

                    var policyData = new PolicyPdfModel
                    {
                        PolicyNumber = model.PolicyNumber,
                        StartDate = model.DepartureDate,
                        EndDate = model.ArrivalDate,
                        InsuredName = currentCompany.RaisonSocial,
                        Address = "N/A",
                        VehicleDescription = $"Transportation: {model.Description} (from {model.Origin} to {model.Destination})",
                        VIN = "N/A",
                        Coverages = new List<CoverageModel>
                        {
                            new CoverageModel { Description = "Cargo Insurance", Deductible = 0, Amount = model.Value }
                        }
                    };
                    await DocumentModule.GenerateAndLinkPolicyConfirmationAsync(currentCompany.Id, "Transportation", policyData, transId);

                    // Mark as insured
                    businessModel.Id = transId;
                    businessModel.IsInsured = true;
                    businessModel.PolicyNumber = model.PolicyNumber;
                    await TransportationModule.SetTransportationAsync(businessModel);
                    
                    model.IsInsured = true;
                }

                _ = Task.Delay(2000).ContinueWith(_ => Navigation.NavigateTo("/transportation-list"));
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