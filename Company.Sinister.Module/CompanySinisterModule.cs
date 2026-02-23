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

        public async Task<CompanySinisterDataModel?> ReadSinisterByIdAsync(long id)
        {
            return await _sinisterProvider.ReadSinisterByIdAsync(id);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadCompanySinistersAsync(long entrepriseId)
        {
            return await _sinisterProvider.ReadSinistersByEntrepriseIdAsync(entrepriseId);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByAssetTypeAsync(long entrepriseId, string assetType)
        {
            return await _sinisterProvider.ReadSinistersByAssetTypeAsync(entrepriseId, assetType);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByFleetAsync(long fleetId)
        {
            return await _sinisterProvider.ReadSinistersByFleetAsync(fleetId);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByTransportationAsync(long transportationId)
        {
            return await _sinisterProvider.ReadSinistersByTransportationAsync(transportationId);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByWarehouseAsync(long warehouseId)
        {
            return await _sinisterProvider.ReadSinistersByWarehouseAsync(warehouseId);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByStatusAsync(long entrepriseId, string status)
        {
            return await _sinisterProvider.ReadSinistersByStatusAsync(entrepriseId, status);
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
