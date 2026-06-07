using Company.Sinister.Module.Data.Models;

namespace Company.Sinister.Module.Data.Providers
{
    public interface ICompanySinisterReadOnlyDataProvider
    {
        Task<CompanySinisterDataModel?> ReadSinisterByIdAsync(long id);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByEntrepriseIdAsync(long entrepriseId);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByAssetTypeAsync(long entrepriseId, string assetType);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByFleetAsync(long fleetId);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByTransportationAsync(long transportationId);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByWarehouseAsync(long warehouseId);
        Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByStatusAsync(long entrepriseId, string status);
    }
}

