using ClientApp.Models;
using ClientApp.Services;
using Company.Warehouse.Module;
using Company.Warehouse.Module.Business;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ClientApp.Components.Pages.Routed
{
    public partial class WarehouseMaterialsComponent : ComponentBase
    {
        [Parameter] public long WarehouseId { get; set; }

        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected AuthenticationService AuthService { get; set; } = default!;
        [Inject] protected ICompanyWarehouseModule WarehouseModule { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

        protected EntrepriseWarehouseBusinessModel? warehouse;
        protected List<WarehouseMaterialViewModel> materials = new();
        protected WarehouseMaterialViewModel materialModel = new();
        protected bool isLoading = true;
        protected bool isSubmitting = false;
        protected long? editingMaterialId;

        protected override async Task OnInitializedAsync()
        {
            if (!AuthService.IsAuthenticated())
            {
                Navigation.NavigateTo("/login");
                return;
            }

            await LoadData();
        }

        protected async Task LoadData()
        {
            isLoading = true;
            try
            {
                warehouse = await WarehouseModule.GetWarehouseAsync(WarehouseId);
                var result = await WarehouseModule.GetWarehouseMaterialsAsync(WarehouseId);
                materials = result.Select(m => new WarehouseMaterialViewModel
                {
                    Id = m.Id,
                    Description = m.Description,
                    ApproximateValue = m.ApproximateValue,
                    WantsInsurance = m.WantsInsurance,
                    IsInsured = m.IsInsured
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading materials: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        protected async Task HandleSubmit()
        {
            isSubmitting = true;
            try
            {
                var businessModel = new EntrepriseWarehouseMaterialBusinessModel
                {
                    Id = editingMaterialId ?? 0,
                    WarehouseId = WarehouseId,
                    Description = materialModel.Description,
                    ApproximateValue = materialModel.ApproximateValue,
                    WantsInsurance = materialModel.WantsInsurance,
                    IsInsured = materialModel.IsInsured
                };

                if (editingMaterialId.HasValue)
                {
                    await WarehouseModule.SetMaterialAsync(businessModel);
                }
                else
                {
                    await WarehouseModule.AddMaterialAsync(businessModel);
                }

                materialModel = new();
                editingMaterialId = null;
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving material: {ex.Message}");
            }
            finally
            {
                isSubmitting = false;
            }
        }

        protected void StartEdit(WarehouseMaterialViewModel m)
        {
            editingMaterialId = m.Id;
            materialModel = new WarehouseMaterialViewModel
            {
                Id = m.Id,
                Description = m.Description,
                ApproximateValue = m.ApproximateValue,
                WantsInsurance = m.WantsInsurance,
                IsInsured = m.IsInsured
            };
        }

        protected void CancelEdit()
        {
            editingMaterialId = null;
            materialModel = new();
        }

        protected async Task DeleteMaterial(WarehouseMaterialViewModel m)
        {
            if (await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {m.Description}?"))
            {
                await WarehouseModule.RemoveMaterialAsync(m.Id);
                await LoadData();
            }
        }
    }
}
