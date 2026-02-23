using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class SinistersList : AuthenticatedComponentBase
    {
        [Inject] protected SinisterListController SinisterListController { get; set; } = default!;

        // --- State (alphabetically sorted) ---
        protected string errorMessage = string.Empty;
        protected string filterStatus = string.Empty;
        protected bool isLoading = true;
        protected bool isProcessing = false;
        protected decimal resolvedAmountInput = 0;
        protected CompanySinisterViewModel? selectedSinister;
        protected List<CompanySinisterViewModel> sinisters = new();
        protected bool showApprovalModal = false;
        protected bool successOperation = false;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadDataAsync();
        }

        #region Private Methods

        private async Task LoadDataAsync()
        {
            isLoading = true;
            errorMessage = string.Empty;
            try
            {
                if (CurrentEnterpriseId.HasValue)
                {
                    sinisters = string.IsNullOrEmpty(filterStatus)
                        ? await SinisterListController.IndexAsync(CurrentEnterpriseId.Value)
                        : await SinisterListController.IndexByStatusAsync(CurrentEnterpriseId.Value, filterStatus);
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error loading claims: {ex.Message}";
            }
            finally
            {
                isLoading = false;
            }
        }

        protected async Task OnFilterChangedAsync(ChangeEventArgs e)
        {
            filterStatus = e.Value?.ToString() ?? string.Empty;
            await LoadDataAsync();
        }

        protected void OpenApprovalModal(CompanySinisterViewModel sinister, bool approve)
        {
            selectedSinister = sinister;
            resolvedAmountInput = approve ? sinister.EstimatedAmount : 0;
            showApprovalModal = true;
            successOperation = approve;
            errorMessage = string.Empty;
        }

        protected void CloseApprovalModal()
        {
            showApprovalModal = false;
            selectedSinister = null;
            errorMessage = string.Empty;
        }

        protected async Task ConfirmApprovalAsync()
        {
            if (selectedSinister == null) return;

            isProcessing = true;
            try
            {
                var approved = successOperation;
                var ok = await SinisterListController.StoreApprovalAsync(
                    selectedSinister.Id,
                    approved,
                    approved ? resolvedAmountInput : null);

                if (ok)
                {
                    CloseApprovalModal();
                    await LoadDataAsync();
                }
                else
                {
                    errorMessage = "Failed to update claim status.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                isProcessing = false;
            }
        }

        protected static string GetStatusBadgeClass(string status) => status switch
        {
            "Pending"    => "badge-pending",
            "InProgress" => "badge-inprogress",
            "Resolved"   => "badge-resolved",
            "Rejected"   => "badge-rejected",
            _            => "badge-pending"
        };

        protected static string GetStatusIcon(string status) => status switch
        {
            "Pending"    => "⏳",
            "InProgress" => "🔄",
            "Resolved"   => "✅",
            "Rejected"   => "❌",
            _            => "⏳"
        };

        #endregion
    }
}
