using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class AddWarehouseComponent : AuthenticatedComponentBase
    {
        // --- Parameters ---
        [Parameter][SupplyParameterFromQuery] public long? Id { get; set; }

        // --- Injections ---
        [Inject] protected WarehouseController Controller { get; set; } = default!;

        // --- State (alphabetically sorted, ID first if any) ---
        protected EntrepriseViewModel? currentCompany;
        protected List<string> errors = new();
        protected bool isLoadingCompany = true;
        protected bool isLoadingItem = false;
        protected bool isSubmitting = false;
        protected List<WarehouseMaterialViewModel> managedMaterials = new();
        protected WarehouseMaterialViewModel newMaterial = new();
        protected string successMessage = string.Empty;
        protected WarehouseViewModel warehouseModel = new();

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            var company = await GetOrLoadCurrentCompanyAsync();
            currentCompany = EntrepriseViewModel.FromBusinessModel(company);
            isLoadingCompany = false;

            if (Id.HasValue && currentCompany != null)
            {
                await LoadWarehouseItem(Id.Value);
            }
        }

        #region Private
        private async Task LoadWarehouseItem(long id)
        {
            isLoadingItem = true;
            try
            {
                var item = await Controller.Show(id, currentCompany?.Id ?? 0);
                if (item != null)
                {
                    warehouseModel = item;
                    managedMaterials = await Controller.GetMaterials(id);
                }
                else
                {
                    errors.Add("Warehouse item not found.");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error loading warehouse data: {ex.Message}");
            }
            finally
            {
                isLoadingItem = false;
            }
        }

        private void AddMaterialToList()
        {
            if (string.IsNullOrWhiteSpace(newMaterial.Description)) return;

            managedMaterials.Add(new WarehouseMaterialViewModel
            {
                Description = newMaterial.Description,
                ApproximateValue = newMaterial.ApproximateValue,
                WantsInsurance = newMaterial.WantsInsurance
            });

            newMaterial = new();
        }

        private void RemoveMaterialFromList(WarehouseMaterialViewModel m)
        {
            managedMaterials.Remove(m);
        }

        private async Task HandleSubmit()
        {
            if (currentCompany == null) return;

            isSubmitting = true;
            errors.Clear();
            successMessage = string.Empty;

            try
            {
                var result = await Controller.Store(
                    warehouseModel,
                    currentCompany.Id,
                    managedMaterials,
                    currentCompany.RaisonSocial);

                if (result.Success)
                {
                    successMessage = result.Message;
                    if (!Id.HasValue)
                    {
                        warehouseModel = new WarehouseViewModel();
                        managedMaterials.Clear();
                    }
                    else
                    {
                        // Refresh data to show isInsured status or insurance policy number
                        await LoadWarehouseItem(Id.Value);
                    }
                }
                else
                {
                    errors.AddRange(result.Errors);
                }

                _ = Task.Delay(3000).ContinueWith(_ =>
                {
                    InvokeAsync(() =>
                    {
                        successMessage = string.Empty;
                        StateHasChanged();
                    });
                });
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