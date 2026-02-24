using ClientApp.Models;
using Company.Warehouse.Module;
using Company.Warehouse.Module.Business;
using CompanyDocuments.Module;

namespace ClientApp.Controllers
{
    public class WarehouseController
    {
        private readonly ICompanyWarehouseModule _warehouseModule;
        private readonly ICompanyDocumentModule _documentModule;

        public WarehouseController(ICompanyWarehouseModule warehouseModule, ICompanyDocumentModule documentModule)
        {
            _warehouseModule = warehouseModule;
            _documentModule = documentModule;
        }

        /// <summary>
        /// REST: Index
        /// </summary>
        public async Task<List<WarehouseViewModel>> Index(long enterpriseId)
        {
            var warehouses = await _warehouseModule.GetCompanyWarehousesAsync(enterpriseId);
            return warehouses.Select(MapBusinessModelToViewModel).ToList();
        }

        /// <summary>
        /// REST: Show
        /// </summary>
        public async Task<WarehouseViewModel?> Show(long id, long enterpriseId)
        {
            var warehouses = await _warehouseModule.GetCompanyWarehousesAsync(enterpriseId);
            var warehouse = warehouses.FirstOrDefault(w => w.Id == id);
            return warehouse != null ? MapBusinessModelToViewModel(warehouse) : null;
        }

        /// <summary>
        /// REST: Store
        /// </summary>
        public async Task<StoreResult> Store(
            WarehouseViewModel viewModel,
            long enterpriseId,
            List<WarehouseMaterialViewModel> materials,
            string? companyRaisonSocial = null)
        {
            var result = new StoreResult();
            try
            {
                var businessModel = MapViewModelToBusinessModel(viewModel, enterpriseId);
                long warehouseId;

                if (viewModel.Id > 0)
                {
                    await _warehouseModule.SetWarehouseAsync(businessModel);
                    warehouseId = viewModel.Id;
                    result.Message = "Warehouse updated successfully!";
                }
                else
                {
                    warehouseId = await _warehouseModule.AddWarehouseAsync(businessModel);
                    businessModel.Id = warehouseId; // Ensure Id is set on business model
                    result.Message = "Warehouse and materials added successfully!";
                }

                result.EmployeeId = warehouseId; // Re-using EmployeeId property for the Id of the saved item

                // Handle materials
                foreach (var m in materials)
                {
                    if (m.Id == 0) // New material
                    {
                        await _warehouseModule.AddMaterialAsync(new EntrepriseWarehouseMaterialBusinessModel
                        {
                            WarehouseId = warehouseId,
                            Description = m.Description,
                            ApproximateValue = m.ApproximateValue,
                            WantsInsurance = m.WantsInsurance
                        });
                    }
                }

                // Policy generation if requested
                if (viewModel.WantsInsurance && !viewModel.IsInsured)
                {
                    // Supprimer les anciennes confirmations non signées pour éviter la redondance
                    await _documentModule.RemoveUnsignedDocumentsForAssetAsync(enterpriseId, warehouseId: warehouseId);
                    
                    // Map view models to business models for generation
                    var materialBusinessModels = materials.Select(m => new EntrepriseWarehouseMaterialBusinessModel
                    {
                        Id = m.Id,
                        Description = m.Description,
                        ApproximateValue = m.ApproximateValue,
                        WantsInsurance = m.WantsInsurance
                    }).ToList();

                    // Générer et lier le document de confirmation d'assurance
                    // L'entrepôt reste en statut "en attente" (WantsInsurance=true, IsInsured=false)
                    // IsInsured ne passera à true que lorsque le document sera SIGNÉ
                    await _documentModule.GenerateAndLinkPolicyConfirmationAsync(enterpriseId, businessModel, materialBusinessModels, companyRaisonSocial ?? "Company");
                    
                    // NE PAS marquer comme assuré automatiquement
                    // Le statut "assuré" sera appliqué uniquement après signature du document
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Failed to save warehouse: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// REST: Destroy
        /// </summary>
        public async Task<bool> Destroy(long id)
        {
            try
            {
                await _warehouseModule.RemoveWarehouseAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<WarehouseMaterialViewModel>> GetMaterials(long warehouseId)
        {
            var materials = await _warehouseModule.GetWarehouseMaterialsAsync(warehouseId);
            return materials.Select(m => new WarehouseMaterialViewModel
            {
                Id = m.Id,
                Description = m.Description,
                ApproximateValue = m.ApproximateValue,
                WantsInsurance = m.WantsInsurance,
                IsInsured = m.IsInsured
            }).ToList();
        }

        #region Mapping
        private WarehouseViewModel MapBusinessModelToViewModel(EntrepriseWarehouseBusinessModel b)
        {
            return new WarehouseViewModel
            {
                Address = b.Address,
                ContentsDescription = b.ContentsDescription,
                EntrepriseId = b.EntrepriseId,
                FranchiseAmount = b.FranchiseAmount,
                FranchisePercentage = b.FranchisePercentage,
                FranchiseType = b.FranchiseType,
                Id = b.Id,
                InsuranceEndDate = b.InsuranceEndDate,
                InsuranceStartDate = b.InsuranceStartDate,
                IsInsured = b.IsInsured,
                Name = b.Name,
                PolicyNumber = b.PolicyNumber,
                SizeM2 = b.SizeM2,
                WantsInsurance = b.WantsInsurance
            };
        }

        private EntrepriseWarehouseBusinessModel MapViewModelToBusinessModel(WarehouseViewModel vm, long enterpriseId)
        {
            return new EntrepriseWarehouseBusinessModel
            {
                Address = vm.Address,
                ContentsDescription = vm.ContentsDescription,
                EntrepriseId = enterpriseId,
                FranchiseAmount = vm.FranchiseAmount,
                FranchisePercentage = vm.FranchisePercentage,
                FranchiseType = vm.FranchiseType,
                Id = vm.Id,
                InsuranceEndDate = vm.InsuranceEndDate,
                InsuranceStartDate = vm.InsuranceStartDate,
                IsInsured = vm.IsInsured,
                Name = vm.Name,
                PolicyNumber = vm.PolicyNumber,
                SizeM2 = vm.SizeM2,
                WantsInsurance = vm.WantsInsurance
            };
        }
        #endregion
    }
}
