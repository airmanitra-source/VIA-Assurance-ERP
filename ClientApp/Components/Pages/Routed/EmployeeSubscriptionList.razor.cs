using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Subscription.Module;
using Subscription.Module.Business;
using Employee.Module;
using Employee.Module.Business;
using ClientApp.Services;

namespace ClientApp.Components.Pages.Routed
{
    public partial class EmployeeSubscriptionList : ComponentBase, IDisposable
    {
        [Parameter]
        public int EmployeeId { get; set; }

        private List<SouscriptionBusinessModel>? subscriptions;
        private EmployeeBusinessModel? employee;
        private bool isLoading = true;
        private System.Timers.Timer? timer;

        [Inject] private AuthenticationService AuthService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private ISubscriptionModule SubscriptionModule { get; set; } = default!;
        [Inject] private IEmployeeModule EmployeeModule { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (!AuthService.IsAuthenticated())
            {
                Navigation.NavigateTo("/login");
                return;
            }

            await LoadData();
            StartTimer();
        }

        private void StartTimer()
        {
            timer = new System.Timers.Timer(30000); // 30 seconds
            timer.Elapsed += async (sender, e) => await TriggerMoqEmployeeSubscriptionFetchingAsync();
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private async Task TriggerMoqEmployeeSubscriptionFetchingAsync()
        {
            try
            {
                if (employee == null) return;

                // Trigger real-time update simulation
                var nextSub = new SouscriptionBusinessModel
                {
                    EmployeeId = EmployeeId,
                    EntrepriseId = employee.EntrepriseID,
                    AnneeCotisation = 2026,
                    MontantCotisation = 50000 // Simulation default amount
                };

                await SubscriptionModule.AddSubscriptionAsync(nextSub);

                // Refresh data on UI thread
                await InvokeAsync(async () =>
                {
                    var result = await SubscriptionModule.GetSubscriptionsByEmployeeIdAsync(EmployeeId);
                    subscriptions = result.ToList();
                    StateHasChanged();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Real-time Simulation] Update failed: {ex.Message}");
            }
        }

        private async Task LoadData()
        {
            isLoading = true;
            try
            {
                employee = await EmployeeModule.GetEmployeeByIdAsync((int)EmployeeId);
                if (employee == null || !employee.VouloirSouscrire)
                {
                    Navigation.NavigateTo("/list-employees");
                    return;
                }
                var result = await SubscriptionModule.GetSubscriptionsByEmployeeIdAsync(EmployeeId);
                subscriptions = result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading subscriptions: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private string GetMonthName(int month)
        {
            return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }

        private void EditSubscription(SouscriptionBusinessModel sub)
        {
            // Future feature: edit subscription
        }

        private async Task RemoveSubscription(SouscriptionBusinessModel sub)
        {
            try
            {
                await SubscriptionModule.RemoveSubscriptionAsync(sub.Id);
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting subscription: {ex.Message}");
            }
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
