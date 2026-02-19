using ClientApp.Models;
using Company.Warehouse.Module;
using Company.Warehouse.Module.Business;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Controllers
{
    public class WarehouseController
    {
        private readonly ICompanyWarehouseModule _warehouseModule;

        public WarehouseController(ICompanyWarehouseModule warehouseModule)
        {
            _warehouseModule = warehouseModule;
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
        public async Task<StoreResult> Store(WarehouseViewModel viewModel, long enterpriseId)
        {
            var result = new StoreResult();
            try
            {
                var businessModel = MapViewModelToBusinessModel(viewModel, enterpriseId);
                if (viewModel.Id > 0)
                {
                    await _warehouseModule.SetWarehouseAsync(businessModel);
                    result.Message = "Warehouse updated successfully!";
                }
                else
                {
                    await _warehouseModule.AddWarehouseAsync(businessModel);
                    result.Message = "Warehouse added successfully!";
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

        #region Mapping
        private WarehouseViewModel MapBusinessModelToViewModel(EntrepriseWarehouseBusinessModel b)
        {
            return new WarehouseViewModel
            {
                Id = b.Id,
                EntrepriseId = b.EntrepriseId,
                Address = b.Address,
                ContentsDescription = b.ContentsDescription,
                IsInsured = b.IsInsured,
                Name = b.Name,
                SizeM2 = b.SizeM2,
                WantsInsurance = b.WantsInsurance
            };
        }

        private EntrepriseWarehouseBusinessModel MapViewModelToBusinessModel(WarehouseViewModel vm, long enterpriseId)
        {
            return new EntrepriseWarehouseBusinessModel
            {
                Id = vm.Id,
                EntrepriseId = enterpriseId,
                Address = vm.Address,
                ContentsDescription = vm.ContentsDescription,
                IsInsured = vm.IsInsured,
                Name = vm.Name,
                SizeM2 = vm.SizeM2,
                WantsInsurance = vm.WantsInsurance
            };
        }
        #endregion
    }
}
