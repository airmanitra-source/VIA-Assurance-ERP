using Company.Warehouse.Module.Business;

namespace Company.Warehouse.Module
{
    public interface ICompanyWarehouseModule
    {
        #region Add
        Task<long> AddWarehouseAsync(EntrepriseWarehouseBusinessModel warehouse);
        Task<long> AddMaterialAsync(EntrepriseWarehouseMaterialBusinessModel material);
        #endregion

        #region Get
        Task<EntrepriseWarehouseMaterialBusinessModel?> GetMaterialAsync(long id);
        Task<IEnumerable<EntrepriseWarehouseMaterialBusinessModel>> GetWarehouseMaterialsAsync(long warehouseId);
        Task<EntrepriseWarehouseBusinessModel?> GetWarehouseAsync(long id);
        Task<IEnumerable<EntrepriseWarehouseBusinessModel>> GetCompanyWarehousesAsync(long entrepriseId);
        #endregion

        #region Set
        Task SetWarehouseAsync(EntrepriseWarehouseBusinessModel warehouse);
        Task SetMaterialAsync(EntrepriseWarehouseMaterialBusinessModel material);
        #endregion

        #region Remove
        Task RemoveWarehouseAsync(long id);
        Task RemoveMaterialAsync(long id);
        #endregion

        #region Insure
        Task MarkAsInsuredAsync(long id);
        #endregion
    }
}
