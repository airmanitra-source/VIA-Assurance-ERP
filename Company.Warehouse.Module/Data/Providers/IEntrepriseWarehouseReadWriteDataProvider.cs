using Company.Warehouse.Module.Data.Models;

namespace Company.Warehouse.Module.Data.Providers
{
    public interface IEntrepriseWarehouseReadWriteDataProvider : IEntrepriseWarehouseReadOnlyDataProvider
    {
        Task<long> AddWarehouseAsync(EntrepriseWarehouseDataModel warehouse);
        Task UpdateWarehouseAsync(EntrepriseWarehouseDataModel warehouse);
        Task DeleteWarehouseAsync(long id);

        Task<long> AddMaterialAsync(EntrepriseWarehouseMaterialDataModel material);
        Task UpdateMaterialAsync(EntrepriseWarehouseMaterialDataModel material);
        Task DeleteMaterialAsync(long id);
    }
}

