using Company.Sinister.Module.Data.Models;

namespace Company.Sinister.Module
{
    public interface ICompanySinisterModule
    {
        Task<CompanySinisterDataModel?> GetSinisterByIdAsync(long id);
        Task<IEnumerable<CompanySinisterDataModel>> GetCompanySinistersAsync(long entrepriseId);
        Task<IEnumerable<CompanySinisterDataModel>> GetSinistersByAssetTypeAsync(long entrepriseId, string assetType);
        Task<IEnumerable<CompanySinisterDataModel>> GetSinistersByFleetAsync(long fleetId);
        Task<IEnumerable<CompanySinisterDataModel>> GetSinistersByTransportationAsync(long transportationId);
        Task<IEnumerable<CompanySinisterDataModel>> GetSinistersByWarehouseAsync(long warehouseId);
        Task<IEnumerable<CompanySinisterDataModel>> GetSinistersByStatusAsync(long entrepriseId, string status);
        Task<long> AddSinisterAsync(CompanySinisterDataModel sinister);
        Task UpdateSinisterAsync(CompanySinisterDataModel sinister);
        Task DeleteSinisterAsync(long id);
    }
}
