using Company.Sinister.Module.Business;

namespace Company.Sinister.Module
{
    public interface ICompanySinisterModule
    {
        Task<long> AddSinisterAsync(CompanySinisterBusinessModel sinister, IReadOnlyList<(string FileName, byte[] FileContent, string TypeDocument)> documents, IEnumerable<long> sinisterTypeIds);

        Task DeleteSinisterAsync(long id);

        Task<IEnumerable<CompanySinisterBusinessModel>> GetCompanySinistersAsync(long entrepriseId);

        Task<CompanySinisterBusinessModel?> GetSinisterByIdAsync(long id);

        Task<IEnumerable<CompanySinisterBusinessModel>> GetSinistersByAssetTypeAsync(long entrepriseId, string assetType);

        Task<IEnumerable<CompanySinisterBusinessModel>> GetSinistersByFleetAsync(long fleetId);

        Task<IEnumerable<CompanySinisterBusinessModel>> GetSinistersByStatusAsync(long entrepriseId, string status);

        Task<IEnumerable<CompanySinisterBusinessModel>> GetSinistersByTransportationAsync(long transportationId);

        Task<IEnumerable<CompanySinisterBusinessModel>> GetSinistersByWarehouseAsync(long warehouseId);

        Task<List<SinisterTypeBusinessModel>> GetSinisterTypesAsync();

        Task<List<SinisterTypeBusinessModel>> GetSinisterTypesBySinisterIdAsync(long sinisterId);

        Task SetSinisterAsync(CompanySinisterBusinessModel sinister);
    }
}
