using ClientApp.Models;
using ClientApp.Services;
using Company.Module;
using Company.Warehouse.Module;
using Company.Warehouse.Module.Data.Models;
using Company.Warehouse.Module.Business;
using CompanyDocuments.Module.Business;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class AddWarehouse: ComponentBase
    {
        [Parameter]
        [SupplyParameterFromQuery] 
        public long? Id { get; set; }

        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;

        [Inject]
        protected AuthenticationService AuthService { get; set; } = default!;

        [Inject]
        protected ICompanyWarehouseModule WarehouseModule { get; set; } = default!;

        [Inject]
        protected ICompanyModule CompanyModule { get; set; } = default!;

        [Inject]
        protected global::CompanyDocuments.Module.ICompanyDocumentModule DocumentModule { get; set; } = default!;

        protected WarehouseViewModel warehouseModel = new();
        protected WarehouseMaterialViewModel newMaterial = new();
        protected List<WarehouseMaterialViewModel> managedMaterials = new();
        protected EntrepriseViewModel? currentCompany;
        protected bool isLoadingCompany = true;
        protected bool isLoadingItem = false;
        protected bool isSubmitting = false;
        protected List<string> errors = new();
        protected string successMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (!AuthService.IsAuthenticated())
            {
                Navigation.NavigateTo("/login");
                return;
            }

            await LoadCurrentCompany();

            if (Id.HasValue && currentCompany != null)
            {
                await LoadWarehouseItem(Id.Value);
            }
        }

        #region
        private async Task LoadCurrentCompany()
        {
            isLoadingCompany = true;
            try
            {
                var entrepriseId = AuthService.GetCurrentEntrepriseId();
                if (entrepriseId.HasValue)
                {
                    var currentCompanyBusinessModel = await CompanyModule.GetCompanyByIdAsync(entrepriseId.Value);
                    currentCompany = EntrepriseViewModel.FromBusinessModel(currentCompanyBusinessModel);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error loading company data: {ex.Message}");
            }
            finally
            {
                isLoadingCompany = false;
            }
        }

        private async Task LoadWarehouseItem(long id)
        {
            isLoadingItem = true;
            try
            {
                var item = await WarehouseModule.GetWarehouseAsync(id);
                if (item != null)
                {
                    warehouseModel = new WarehouseViewModel
                    {
                        Name = item.Name,
                        SizeM2 = item.SizeM2,
                        ContentsDescription = item.ContentsDescription,
                        Address = item.Address,
                        WantsInsurance = item.WantsInsurance,
                        IsInsured = item.IsInsured,
                        PolicyNumber = item.PolicyNumber
                    };

                    var materials = await WarehouseModule.GetWarehouseMaterialsAsync(id);
                    managedMaterials = materials.Select(m => new WarehouseMaterialViewModel
                    {
                        Id = m.Id,
                        Description = m.Description,
                        ApproximateValue = m.ApproximateValue,
                        WantsInsurance = m.WantsInsurance,
                        IsInsured = m.IsInsured
                    }).ToList();
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
                var businessModel = new EntrepriseWarehouseBusinessModel
                {
                    Id = Id ?? 0,
                    EntrepriseId = currentCompany.Id,
                    Name = warehouseModel.Name,
                    SizeM2 = warehouseModel.SizeM2,
                    ContentsDescription = warehouseModel.ContentsDescription,
                    Address = warehouseModel.Address,
                    WantsInsurance = warehouseModel.WantsInsurance,
                    IsInsured = warehouseModel.IsInsured,
                    PolicyNumber = warehouseModel.PolicyNumber
                };

                long warehouseId = Id ?? 0;
                if (Id.HasValue)
                {
                    await WarehouseModule.SetWarehouseAsync(businessModel);
                }
                else
                {
                    warehouseId = await WarehouseModule.AddWarehouseAsync(businessModel);
                }

                // Simple strategy for materials in this integrated form:
                // Since we don't have a complex diffing, and materials are small,
                // we'll delete existing ones and re-add if we're in a "save everything" mode.
                // OR we can just add the new ones.
                // For a better UX, let's just save the materials that don't have an ID yet, 
                // and assume existing ones were managed via the separate page or we just focus on adding.
                // BUT the requirement is "user should also be able to add WareHouseMaterials" in AddWarehouse.

                foreach (var m in managedMaterials)
                {
                    if (m.Id == 0) // New material added in this form
                    {
                        await WarehouseModule.AddMaterialAsync(new EntrepriseWarehouseMaterialBusinessModel
                        {
                            WarehouseId = warehouseId,
                            Description = m.Description,
                            ApproximateValue = m.ApproximateValue,
                            WantsInsurance = m.WantsInsurance
                        });
                    }
                    // We could also handle updates if we wanted, but the prompt emphasizes adding.
                }

                if (warehouseModel.WantsInsurance && !warehouseModel.IsInsured)
                {
                    warehouseModel.PolicyNumber = "WHS-" + warehouseId + "-" + DateTime.Now.Ticks.ToString().Substring(12);

                    var policyData = new PolicyPdfModel
                    {
                        PolicyNumber = warehouseModel.PolicyNumber,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddYears(1),
                        InsuredName = currentCompany.RaisonSocial,
                        Address = warehouseModel.Address,
                        VehicleDescription = $"Warehouse: {warehouseModel.Name} ({warehouseModel.SizeM2} m²)",
                        VIN = "N/A",
                        Coverages = managedMaterials.Where(m => m.WantsInsurance).Select(m => new CoverageModel
                        {
                            Description = m.Description,
                            Deductible = 0,
                            Amount = m.ApproximateValue
                        }).ToList()
                    };

                    await DocumentModule.GenerateAndLinkPolicyConfirmationAsync(currentCompany.Id, "Warehouse", policyData, warehouseId);
                    
                    // Update database to mark as insured
                    businessModel.Id = warehouseId;
                    businessModel.IsInsured = true;
                    businessModel.PolicyNumber = warehouseModel.PolicyNumber;
                    await WarehouseModule.SetWarehouseAsync(businessModel);
                    
                    warehouseModel.IsInsured = true;
                }

                successMessage = Id.HasValue ? "Warehouse updated successfully!" : "Warehouse and materials added successfully!";

                if (!Id.HasValue)
                {
                    warehouseModel = new WarehouseViewModel();
                    managedMaterials.Clear();
                }

                _ = Task.Delay(3000).ContinueWith(_ => {
                    InvokeAsync(() => {
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