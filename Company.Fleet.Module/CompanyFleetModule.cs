using Company.Fleet.Module.Business;
using Company.Fleet.Module.Data.Providers;

namespace Company.Fleet.Module
{
    public class CompanyFleetModule : ICompanyFleetModule
    {
        private readonly IEntrepriseFleetReadWrite _fleetProvider;

        public CompanyFleetModule(IEntrepriseFleetReadWrite fleetProvider)
        {
            _fleetProvider = fleetProvider;
        }

        public async Task<EntrepriseFleetBusinessModel?> GetFleetItemAsync(long id)
        {
            var fleetDataModel = await _fleetProvider.GetFleetItemByIdAsync(id);
            return EntrepriseFleetBusinessModel.FromDataModel(fleetDataModel);
        }

        public async Task<List<EntrepriseFleetBusinessModel>> GetCompanyFleetAsync(long entrepriseId)
        {
            var fleetsDataModel = await _fleetProvider.GetFleetByEntrepriseIdAsync(entrepriseId);
            return fleetsDataModel.Select(EntrepriseFleetBusinessModel.FromDataModel).ToList();
        }

        public async Task<long> AddFleetItemAsync(EntrepriseFleetBusinessModel fleetItem)
        {
            var fleetItemDataModel = fleetItem.ToDataModel();
            return await _fleetProvider.AddFleetItemAsync(fleetItemDataModel);
        }

        public async Task SetFleetItemAsync(EntrepriseFleetBusinessModel fleetItem)
        {
            var fleetItemDataModel = fleetItem.ToDataModel();
            await _fleetProvider.UpdateFleetItemAsync(fleetItemDataModel);
        }

        public async Task RemoveFleetItemAsync(long id)
        {
            await _fleetProvider.DeleteFleetItemAsync(id);
        }
    }
}
