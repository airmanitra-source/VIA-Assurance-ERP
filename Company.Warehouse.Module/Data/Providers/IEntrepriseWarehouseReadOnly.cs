using Company.Warehouse.Module.Data.Models;

namespace Company.Warehouse.Module.Data.Providers
{
    public interface IEntrepriseWarehouseReadOnly
    {
        Task<EntrepriseWarehouseDataModel?> GetWarehouseByIdAsync(long id);
        Task<IEnumerable<EntrepriseWarehouseDataModel>> GetWarehousesByEntrepriseIdAsync(long entrepriseId);
        Task<EntrepriseWarehouseMaterialDataModel?> GetMaterialByIdAsync(long id);
        Task<IEnumerable<EntrepriseWarehouseMaterialDataModel>> GetMaterialsByWarehouseIdAsync(long warehouseId);
    }
}
