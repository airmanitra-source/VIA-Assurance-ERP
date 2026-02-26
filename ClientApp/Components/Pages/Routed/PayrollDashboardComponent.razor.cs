using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class PayrollDashboardComponent : AuthenticatedComponentBase
    {
        [Inject] protected PayrollController Controller { get; set; } = default!;

        protected EntrepriseViewModel? currentCompany;
        protected List<EmployeeViewModel> employees = new();
        protected List<string> errors = new();
        protected HashSet<int> expandedPaySlipIds = new();
        protected string activeTab = "draft";
        protected bool isGenerating = false;
        protected bool isLoading = true;
        protected bool isLoadingSaved = false;
        protected bool isSavingEdits = false;
        protected bool isSubmittingDraft = false;
        protected bool isPaySlipSaved = false;
        protected DateTime newPeriodEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        protected DateTime newPeriodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        protected PaySlipInputViewModel paySlipInput = new();
        protected PaySlipViewModel? paySlipPreview;
        protected List<PayrollPeriodViewModel> periods = new();
        protected List<PaySlipViewModel> savedPaySlips = new();
        protected long? selectedEmployeeId;
        protected int? selectedPeriodId;
        protected string successMessage = string.Empty;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadDataAsync();
        }

        protected async Task CreatePeriodAsync()
        {
            if (currentCompany == null) return;

            try
            {
                var periodId = await Controller.Store(currentCompany.Id, newPeriodStart, newPeriodEnd);
                successMessage = "Période de paie créée avec succès";
                await LoadPeriodsAsync();
                selectedPeriodId = periodId;
                await OnPeriodSelectedAsync();
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
        }

        protected async Task GeneratePreviewAsync()
        {
            if (currentCompany == null || !selectedEmployeeId.HasValue || !selectedPeriodId.HasValue) return;

            isGenerating = true;
            isPaySlipSaved = false;
            try
            {
                paySlipPreview = await Controller.ShowPreviewAsync(
                    selectedEmployeeId.Value,
                    selectedPeriodId.Value,
                    currentCompany.Id,
                    paySlipInput);
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
            finally
            {
                isGenerating = false;
            }
        }

        protected void OnEmployeeSelected(ChangeEventArgs e)
        {
            if (long.TryParse(e.Value?.ToString(), out var id))
            {
                selectedEmployeeId = id;
                var emp = employees.FirstOrDefault(x => x.EmployeeID == id);
                paySlipInput = new PaySlipInputViewModel
                {
                    EmployeeID = id,
                    EmployeeName = emp != null ? $"{emp.Prenom} {emp.Nom}" : string.Empty
                };
                paySlipPreview = null;
                isPaySlipSaved = false;
            }
        }

        protected async Task SavePaySlipAsync()
        {
            if (currentCompany == null || !selectedEmployeeId.HasValue || !selectedPeriodId.HasValue) return;

            try
            {
                await Controller.StorePaySlipAsync(
                    selectedEmployeeId.Value,
                    selectedPeriodId.Value,
                    currentCompany.Id,
                    paySlipInput);
                successMessage = "Bulletin de paie enregistré avec succès";
                isPaySlipSaved = true;

                if (selectedPeriodId.HasValue)
                {
                    await LoadSavedPaySlipsAsync();
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
        }

        protected async Task SubmitDraftToEmployeeAsync()
        {
            if (currentCompany == null || !selectedEmployeeId.HasValue || !selectedPeriodId.HasValue || paySlipPreview == null)
                return;

            isSubmittingDraft = true;
            errors.Clear();
            successMessage = string.Empty;

            try
            {
                await Controller.SubmitDraftToEmployeeAsync(
                    selectedEmployeeId.Value,
                    selectedPeriodId.Value,
                    currentCompany.Id);
                successMessage = $"Bulletin envoyé par email à {paySlipPreview.EmployeeName}. L'employé sera notifié de vérifier son portail.";
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur lors de l'envoi: {ex.Message}");
            }
            finally
            {
                isSubmittingDraft = false;
            }
        }

        protected async Task SetActiveTabAsync(string tab)
        {
            activeTab = tab;
            successMessage = string.Empty;
            errors.Clear();

            if (activeTab == "saved" && selectedPeriodId.HasValue)
            {
                await LoadSavedPaySlipsAsync();
            }
        }

        protected Task SetDraftTabAsync()
        {
            return SetActiveTabAsync("draft");
        }

        protected Task SetSavedTabAsync()
        {
            return SetActiveTabAsync("saved");
        }

        protected void TogglePaySlipExpansion(int payrollId)
        {
            if (!expandedPaySlipIds.Add(payrollId))
            {
                expandedPaySlipIds.Remove(payrollId);
            }
        }

        protected bool IsPaySlipExpanded(int payrollId)
        {
            return expandedPaySlipIds.Contains(payrollId);
        }

        protected static bool IsPaySlipLineEditable(PaySlipLineViewModel line)
        {
            return line.LineType == "Gain";
        }

        protected async Task SaveEditedPaySlipAsync(PaySlipViewModel paySlip)
        {
            if (currentCompany == null || !selectedPeriodId.HasValue)
                return;

            isSavingEdits = true;
            errors.Clear();
            successMessage = string.Empty;

            try
            {
                await Controller.StoreSavedPaySlipAsync(currentCompany.Id, selectedPeriodId.Value, paySlip);
                successMessage = $"Bulletin mis à jour pour {paySlip.EmployeeName}.";
                await LoadSavedPaySlipsAsync();
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur lors de la mise à jour: {ex.Message}");
            }
            finally
            {
                isSavingEdits = false;
            }
        }

        protected async Task OnPeriodSelectedAsync()
        {
            paySlipPreview = null;
            isPaySlipSaved = false;

            if (activeTab == "saved" && selectedPeriodId.HasValue)
            {
                await LoadSavedPaySlipsAsync();
                return;
            }

            savedPaySlips.Clear();
        }

        protected Task OpenSavedPaySlipAsync(PaySlipViewModel paySlip)
        {
            selectedEmployeeId = paySlip.EmployeeID;
            selectedPeriodId = paySlip.PeriodID;
            paySlipPreview = paySlip;
            paySlipInput = new PaySlipInputViewModel
            {
                EmployeeID = paySlip.EmployeeID,
                EmployeeName = paySlip.EmployeeName
            };
            isPaySlipSaved = true;
            activeTab = "draft";
            return Task.CompletedTask;
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;
            try
            {
                var company = await GetOrLoadCurrentCompanyAsync();
                if (company != null)
                {
                    currentCompany = EntrepriseViewModel.FromBusinessModel(company);
                    await Task.WhenAll(LoadPeriodsAsync(), LoadEmployeesAsync());
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

        private async Task LoadEmployeesAsync()
        {
            if (currentCompany == null) return;
            employees = await Controller.IndexEmployeesAsync(currentCompany.Id);
        }

        private async Task LoadPeriodsAsync()
        {
            if (currentCompany == null) return;
            periods = await Controller.Index(currentCompany.Id);
        }

        private async Task LoadSavedPaySlipsAsync()
        {
            if (currentCompany == null || !selectedPeriodId.HasValue)
                return;

            isLoadingSaved = true;
            try
            {
                savedPaySlips = await Controller.IndexSavedPaySlipsAsync(currentCompany.Id, selectedPeriodId.Value);
                expandedPaySlipIds.Clear();
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur: {ex.Message}");
            }
            finally
            {
                isLoadingSaved = false;
            }
        }
    }
}
