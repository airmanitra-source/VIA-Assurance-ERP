using Company.Warehouse.Module.Business;
using Company.Warehouse.Module.Data.Models;
using Company.Warehouse.Module.Data.Providers;

namespace Company.Warehouse.Module
{
    public class CompanyWarehouseModule : ICompanyWarehouseModule
    {
        private readonly IEntrepriseWarehouseReadWriteDataProvider _warehouseProvider;

        public CompanyWarehouseModule(IEntrepriseWarehouseReadWriteDataProvider warehouseProvider)
        {
            _warehouseProvider = warehouseProvider;
        }

        public async Task<EntrepriseWarehouseBusinessModel?> GetWarehouseAsync(long id)
        {
            var dataModel = await _warehouseProvider.GetWarehouseByIdAsync(id);
            return EntrepriseWarehouseBusinessModel.FromDataModel(dataModel);
        }

        public async Task<IEnumerable<EntrepriseWarehouseBusinessModel>> GetCompanyWarehousesAsync(long entrepriseId)
        {
            var dataModels = await _warehouseProvider.GetWarehousesByEntrepriseIdAsync(entrepriseId);
            return dataModels.Select(EntrepriseWarehouseBusinessModel.FromDataModel)!;
        }

        public async Task<long> AddWarehouseAsync(EntrepriseWarehouseBusinessModel warehouse)
        {
            return await _warehouseProvider.AddWarehouseAsync(warehouse.ToDataModel());
        }

        public async Task SetWarehouseAsync(EntrepriseWarehouseBusinessModel warehouse)
        {
            await _warehouseProvider.UpdateWarehouseAsync(warehouse.ToDataModel());
        }

        public async Task RemoveWarehouseAsync(long id)
        {
            // Protection: vÃ©rifier si le warehouse est assurÃ© avant de permettre la suppression
            var warehouse = await _warehouseProvider.GetWarehouseByIdAsync(id);
            
            if (warehouse != null && warehouse.IsInsured)
            {
                throw new InvalidOperationException(
                    "Impossible de supprimer cet entrepÃ´t car il est assurÃ©. " +
                    "La confirmation d'assurance a Ã©tÃ© signÃ©e et l'entrepÃ´t ne peut plus Ãªtre supprimÃ©.");
            }

            await _warehouseProvider.DeleteWarehouseAsync(id);
        }

        public async Task MarkAsInsuredAsync(long id)
        {
            var warehouse = await _warehouseProvider.GetWarehouseByIdAsync(id);
            
            if (warehouse == null)
            {
                throw new InvalidOperationException($"EntrepÃ´t avec ID {id} introuvable.");
            }

            if (warehouse.IsInsured)
            {
                // DÃ©jÃ  assurÃ©, pas besoin de mettre Ã  jour
                return;
            }

            warehouse.IsInsured = true;
            await _warehouseProvider.UpdateWarehouseAsync(warehouse);
        }

        public async Task<EntrepriseWarehouseMaterialBusinessModel?> GetMaterialAsync(long id)
        {
            var dataModel = await _warehouseProvider.GetMaterialByIdAsync(id);
            return EntrepriseWarehouseMaterialBusinessModel.FromDataModel(dataModel);
        }

        public async Task<IEnumerable<EntrepriseWarehouseMaterialBusinessModel>> GetWarehouseMaterialsAsync(long warehouseId)
        {
            var dataModels = await _warehouseProvider.GetMaterialsByWarehouseIdAsync(warehouseId);
            return dataModels.Select(EntrepriseWarehouseMaterialBusinessModel.FromDataModel)!;
        }

        public async Task<long> AddMaterialAsync(EntrepriseWarehouseMaterialBusinessModel material)
        {
            return await _warehouseProvider.AddMaterialAsync(material.ToDataModel());
        }

        public async Task SetMaterialAsync(EntrepriseWarehouseMaterialBusinessModel material)
        {
            await _warehouseProvider.UpdateMaterialAsync(material.ToDataModel());
        }

        public async Task RemoveMaterialAsync(long id)
        {
            await _warehouseProvider.DeleteMaterialAsync(id);
        }
    }
}

