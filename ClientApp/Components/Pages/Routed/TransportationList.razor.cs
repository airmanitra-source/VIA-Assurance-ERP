using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;

namespace ClientApp.Components.Pages.Routed
{
    public partial class TransportationList: AuthenticatedComponentBase
    {
        [Inject] public TransportationController TransportationController { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private List<TransportationViewModel>? transportations;
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
                    transportations = await TransportationController.Index(entrepriseId.Value);
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

        private async Task DeleteTransportation(TransportationViewModel t)
        {
            if (await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {t.Description}?"))
            {
                await TransportationController.Destroy(t.Id);
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