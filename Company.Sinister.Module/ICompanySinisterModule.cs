using Company.Sinister.Module.Data.Models;

namespace Company.Sinister.Module
{
    public interface ICompanySinisterModule
    {
        Task<CompanySinisterDataModel?> ReadSinisterByIdAsync(long id);
        Task<IEnumerable<CompanySinisterDataModel>> ReadCompanySinistersAsync(long entrepriseId);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByAssetTypeAsync(long entrepriseId, string assetType);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByFleetAsync(long fleetId);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByTransportationAsync(long transportationId);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByWarehouseAsync(long warehouseId);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByStatusAsync(long entrepriseId, string status);
        Task<long> AddSinisterAsync(CompanySinisterDataModel sinister);
        Task UpdateSinisterAsync(CompanySinisterDataModel sinister);
        Task DeleteSinisterAsync(long id);
    }
}
