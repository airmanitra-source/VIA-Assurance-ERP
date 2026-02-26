using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class EmployeeDashboardComponent : AuthenticatedComponentBase
    {
        [Inject] protected EmployeeDashboardController DashboardController { get; set; } = default!;

        protected string activeTab = "profile";
        protected List<PayrollPeriodViewModel>? draftPeriods;
        protected PaySlipModificationRequestViewModel? existingModifRequest;
        protected bool isLoading = true;
        protected bool isSubmitting = false;
        protected bool isSubmittingModif = false;
        protected string modifComments = string.Empty;
        protected PaySlipInputViewModel modifInput = new();
        protected string modifMessage = string.Empty;
        protected bool modifSuccess;
        protected EmployeeTimesheetViewModel newTimesheet = new();
        protected List<EmployeePayrollViewModel>? payrolls;
        protected List<PaySlipViewModel>? paySlips;
        protected EmployeeViewModel? profile;
        protected int selectedModifPeriodId;
        protected string timesheetMessage = string.Empty;
        protected bool timesheetSuccess;
        protected List<EmployeeTimesheetViewModel>? timesheets;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadDataAsync();
        }

        protected async Task HandleTimesheetSubmitAsync()
        {
            if (profile == null) return;

            isSubmitting = true;
            timesheetMessage = string.Empty;

            var (success, message) = await DashboardController.StoreTimesheetAsync(newTimesheet, profile.EmployeeID);

            timesheetSuccess = success;
            timesheetMessage = message;
            isSubmitting = false;

            if (success)
            {
                newTimesheet = new EmployeeTimesheetViewModel();
                timesheets = await DashboardController.IndexTimesheetAsync(profile.EmployeeID);
            }
        }

        protected async Task OnModifPeriodChangedAsync()
        {
            modifInput = new PaySlipInputViewModel();
            modifMessage = string.Empty;
            existingModifRequest = null;

            if (selectedModifPeriodId > 0 && profile != null)
            {
                // Pre-fill from saved payslip values
                modifInput = await DashboardController.ShowPaySlipInputAsync(
                    profile.EmployeeID, selectedModifPeriodId, profile.EntrepriseId);

                // Override with existing modification request if present
                existingModifRequest = await DashboardController.ShowModificationRequestAsync(
                    profile.EmployeeID, selectedModifPeriodId);

                if (existingModifRequest != null)
                {
                    modifInput.Bonus = existingModifRequest.Bonus ?? modifInput.Bonus;
                    modifInput.IndemniteLogement = existingModifRequest.IndemniteLogement ?? modifInput.IndemniteLogement;
                    modifInput.IndemniteTransport = existingModifRequest.IndemniteTransport ?? modifInput.IndemniteTransport;
                    modifInput.OvertimeHours = existingModifRequest.OvertimeHours ?? modifInput.OvertimeHours;
                    modifInput.PrimeScolarite = existingModifRequest.PrimeScolarite ?? modifInput.PrimeScolarite;
                    modifInput.TreiziemeMois = existingModifRequest.TreiziemeMois ?? modifInput.TreiziemeMois;
                }
            }
        }

        protected async Task SubmitModificationRequestAsync()
        {
            if (profile == null || selectedModifPeriodId <= 0) return;

            isSubmittingModif = true;
            modifMessage = string.Empty;

            var (success, message) = await DashboardController.StoreModificationRequestAsync(
                modifInput, profile.EmployeeID, selectedModifPeriodId, modifComments);

            modifSuccess = success;
            modifMessage = message;
            isSubmittingModif = false;

            if (success)
            {
                existingModifRequest = await DashboardController.ShowModificationRequestAsync(
                    profile.EmployeeID, selectedModifPeriodId);
            }
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;
            try
            {
                var email = CurrentUserEmail;
                if (string.IsNullOrEmpty(email)) return;

                profile = await DashboardController.ShowProfileAsync(email);

                if (profile != null)
                {
                    timesheets = await DashboardController.IndexTimesheetAsync(profile.EmployeeID);
                    payrolls = await DashboardController.IndexPayrollAsync(profile.EmployeeID);
                    paySlips = await DashboardController.IndexPaySlipsAsync(profile.EmployeeID, profile.EntrepriseId);
                    draftPeriods = await DashboardController.IndexPeriodsAsync(profile.EntrepriseId);

                    if (draftPeriods != null && draftPeriods.Any())
                    {
                        selectedModifPeriodId = draftPeriods.First().PeriodID;
                        await OnModifPeriodChangedAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading employee dashboard: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
