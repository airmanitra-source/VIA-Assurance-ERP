using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ClientApp.Components.Shared;
using Company.Transportation.Module;
using Company.Transportation.Module.Business;

namespace ClientApp.Components.Pages.Routed
{
    public partial class TransportationList: AuthenticatedComponentBase
    {
        [Inject] public ICompanyTransportationModule TransportationModule { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private List<EntrepriseMerchandiseTransportationBusinessModel>? transportations;
        private bool isLoading = true;
        private Company.Module.Business.EntrepriseBusinessModel? currentCompany;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;
            try
            {
                var entrepriseId = AuthService.GetCurrentEntrepriseId();
                if (entrepriseId.HasValue)
                {
                    currentCompany = await CompanyModule.GetCompanyByIdAsync(entrepriseId.Value);
                    if (currentCompany != null)
                    {
                        var result = await TransportationModule.GetCompanyTransportationsAsync(currentCompany.Id);
                        transportations = result.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading transportations: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task DeleteTransportation(EntrepriseMerchandiseTransportationBusinessModel t)
        {
            if (await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {t.Description}?"))
            {
                await TransportationModule.RemoveTransportationAsync(t.Id);
                await LoadDataAsync();
            }
        }

        private string GetFrequencyLabel(string frequency)
        {
            return frequency switch
            {
                "OneTime" => Localizer["FrequencyOneTime"],
                "Weekly" => Localizer["FrequencyWeekly"],
                "Monthly" => Localizer["FrequencyMonthly"],
                "Casual" => Localizer["FrequencyCasual"],
                _ => frequency
            };
        }
    }
}