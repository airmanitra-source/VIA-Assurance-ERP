using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class PayrollSettingsComponent : AuthenticatedComponentBase
    {
        [Inject] protected CompanyPayrollSettingsController _settingsController { get; set; } = default!;

        protected List<string> errors = new();
        protected bool isLoading = true;
        protected bool isSaving = false;
        protected CompanyPayrollSettingsViewModel? settings;
        protected string successMessage = string.Empty;

        // Percentage properties for rate inputs
        protected double cnapsEmployeeRatePercent = 0;
        protected double cnapsComplementaryRatePercent = 0;
        protected double ostieEmployeeRatePercent = 0;
        protected double cnapsEmployerRatePercent = 0;
        protected double cnapsComplementaryEmployerRatePercent = 0;
        protected double ostieEmployerRatePercent = 0;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadSettingsAsync();
        }

        protected async Task SaveSettingsAsync()
        {
            if (settings == null) return;

            isSaving = true;
            errors.Clear();
            successMessage = string.Empty;

            try
            {
                // Convert percentages back to decimals
                settings.CnapsEmployeeRate = (decimal)(cnapsEmployeeRatePercent / 100);
                settings.CnapsComplementaryRate = (decimal)(cnapsComplementaryRatePercent / 100);
                settings.OstieEmployeeRate = (decimal)(ostieEmployeeRatePercent / 100);
                settings.CnapsEmployerRate = (decimal)(cnapsEmployerRatePercent / 100);
                settings.CnapsComplementaryEmployerRate = (decimal)(cnapsComplementaryEmployerRatePercent / 100);
                settings.OstieEmployerRate = (decimal)(ostieEmployerRatePercent / 100);

                await _settingsController.Store(settings);
                successMessage = Localizer["SettingsSavedSuccessfully"];
                await LoadSettingsAsync();
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task LoadSettingsAsync()
        {
            isLoading = true;
            try
            {
                var enterpriseId = CurrentEnterpriseId;
                if (enterpriseId.HasValue)
                {
                    settings = await _settingsController.Show(enterpriseId.Value);
                    settings.EntrepriseID = enterpriseId.Value;

                    // Convert decimals to percentages for display
                    cnapsEmployeeRatePercent = (double)(settings.CnapsEmployeeRate * 100);
                    cnapsComplementaryRatePercent = (double)(settings.CnapsComplementaryRate * 100);
                    ostieEmployeeRatePercent = (double)(settings.OstieEmployeeRate * 100);
                    cnapsEmployerRatePercent = (double)(settings.CnapsEmployerRate * 100);
                    cnapsComplementaryEmployerRatePercent = (double)(settings.CnapsComplementaryEmployerRate * 100);
                    ostieEmployerRatePercent = (double)(settings.OstieEmployerRate * 100);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
