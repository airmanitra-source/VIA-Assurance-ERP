using Company.Sinister.Module.Data.Models;
using Company.Sinister.Module.Data.Providers;

namespace Company.Sinister.Module
{
    public class CompanySinisterModule : ICompanySinisterModule
    {
        private readonly ICompanySinisterReadWrite _sinisterProvider;

        public CompanySinisterModule(ICompanySinisterReadWrite sinisterProvider)
        {
            _sinisterProvider = sinisterProvider;
        }

        public async Task<CompanySinisterDataModel?> GetSinisterAsync(long id)
        {
            return await _sinisterProvider.GetSinisterByIdAsync(id);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> GetCompanySinistersAsync(long entrepriseId)
        {
            return await _sinisterProvider.GetSinistersByEntrepriseIdAsync(entrepriseId);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> GetSinistersByAssetTypeAsync(long entrepriseId, string assetType)
        {
            return await _sinisterProvider.GetSinistersByAssetTypeAsync(entrepriseId, assetType);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> GetSinistersByFleetAsync(long fleetId)
        {
            return await _sinisterProvider.GetSinistersByFleetAsync(fleetId);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> GetSinistersByTransportationAsync(long transportationId)
        {
            return await _sinisterProvider.GetSinistersByTransportationAsync(transportationId);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> GetSinistersByWarehouseAsync(long warehouseId)
        {
            return await _sinisterProvider.GetSinistersByWarehouseAsync(warehouseId);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> GetSinistersByStatusAsync(long entrepriseId, string status)
        {
            return await _sinisterProvider.GetSinistersByStatusAsync(entrepriseId, status);
        }

        public async Task<long> AddSinisterAsync(CompanySinisterDataModel sinister)
        {
            sinister.CreatedDate = DateTime.UtcNow;
            sinister.LastModifiedDate = DateTime.UtcNow;
            return await _sinisterProvider.AddSinisterAsync(sinister);
        }

        public async Task UpdateSinisterAsync(CompanySinisterDataModel sinister)
        {
            sinister.LastModifiedDate = DateTime.UtcNow;
            await _sinisterProvider.UpdateSinisterAsync(sinister);
        }

        public async Task DeleteSinisterAsync(long id)
        {
            await _sinisterProvider.DeleteSinisterAsync(id);
        }
    }
}
