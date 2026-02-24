using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class AddTransportationComponent : AuthenticatedComponentBase
    {
        // --- Parameters ---
        [Parameter][SupplyParameterFromQuery] public long? Id { get; set; }

        // --- Injections ---
        [Inject] protected TransportationController Controller { get; set; } = default!;

        // --- State (alphabetically sorted, ID first if any) ---
        protected EntrepriseViewModel? currentCompany;
        protected List<string> errors = new();
        protected bool isLoadingCompany = true;
        protected bool isLoadingItem = false;
        protected bool isSubmitting = false;
        protected TransportationViewModel model = new();
        protected string successMessage = string.Empty;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            var company = await GetOrLoadCurrentCompanyAsync();
            currentCompany = EntrepriseViewModel.FromBusinessModel(company);
            isLoadingCompany = false;

            if (Id.HasValue && currentCompany != null)
            {
                await LoadItem(Id.Value);
            }
        }

        #region Private
        private async Task LoadItem(long id)
        {
            isLoadingItem = true;
            try
            {
                var item = await Controller.Show(id, currentCompany?.Id ?? 0);
                if (item != null)
                {
                    model = item;
                }
                else
                {
                    errors.Add("Transportation item not found.");
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
                var result = await Controller.Store(model, currentCompany.Id, currentCompany.RaisonSocial);

                if (result.Success)
                {
                    successMessage = result.Message;
                    if (!Id.HasValue)
                    {
                        model = new TransportationViewModel();
                    }
                    else
                    {
                        // Refresh data to show isInsured status or insurance policy number
                        await LoadItem(Id.Value);
                    }
                }
                else
                {
                    errors.AddRange(result.Errors);
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