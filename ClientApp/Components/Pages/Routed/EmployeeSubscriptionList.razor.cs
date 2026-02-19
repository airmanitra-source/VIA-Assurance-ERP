using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ClientApp.Services;
using ClientApp.Controllers;
using ClientApp.Models;

namespace ClientApp.Components.Pages.Routed
{
    public partial class EmployeeSubscriptionList : ComponentBase, IDisposable
    {
        [Parameter]
        public long EmployeeId { get; set; }

        private List<SubscriptionViewModel>? subscriptions;
        private EmployeeViewModel? employee;
        private bool isLoading = true;
        private System.Timers.Timer? timer;

        [Inject] private AuthenticationService AuthService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private SubscriptionController SubscriptionController { get; set; } = default!;
        [Inject] private EmployeeController EmployeeController { get; set; } = default!;

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
                var nextSub = new SubscriptionViewModel
                {
                    EmployeeId = (int)EmployeeId,
                    EntrepriseId = employee.EntrepriseId, // Note: I should check if EntrepriseId is in EmployeeViewModel
                    AnneeCotisation = 2026,
                    MontantCotisation = 50000 // Simulation default amount
                };

                await SubscriptionController.Store(nextSub);

                // Refresh data on UI thread
                await InvokeAsync(async () =>
                {
                    subscriptions = await SubscriptionController.Index(EmployeeId);
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
                var entrepriseId = AuthService.GetCurrentEntrepriseId();
                if (!entrepriseId.HasValue) 
                {
                    Navigation.NavigateTo("/login");
                    return;
                }

                var detail = await EmployeeController.Show(EmployeeId, entrepriseId.Value);
                employee = detail?.Employee;

                if (employee == null || !employee.VouloirSouscrire)
                {
                    Navigation.NavigateTo("/list-employees");
                    return;
                }

                subscriptions = await SubscriptionController.Index(EmployeeId);
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

        private void EditSubscription(SubscriptionViewModel sub)
        {
            // Future feature: edit subscription
        }

        private async Task RemoveSubscription(SubscriptionViewModel sub)
        {
            try
            {
                await SubscriptionController.Destroy(sub.Id);
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
