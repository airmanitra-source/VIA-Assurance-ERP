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
        protected bool isLoading = true;
        protected bool isSubmitting = false;
        protected EmployeeTimesheetViewModel newTimesheet = new();
        protected List<EmployeePayrollViewModel>? payrolls;
        protected List<PaySlipViewModel>? paySlips;
        protected EmployeeViewModel? profile;
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
