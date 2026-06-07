using Company.Fleet.Module.Business.Models;
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
            // Protection: vérifier si le véhicule est assuré avant de permettre la suppression
            var fleetItem = await _fleetProvider.GetFleetItemByIdAsync(id);
            
            if (fleetItem != null && fleetItem.IsInsured)
            {
                throw new InvalidOperationException(
                    "Impossible de supprimer ce véhicule car il est assuré. " +
                    "La confirmation d'assurance a été signée et le véhicule ne peut plus être supprimé.");
            }

            await _fleetProvider.DeleteFleetItemAsync(id);
        }

        public async Task MarkAsInsuredAsync(long id)
        {
            var fleetItem = await _fleetProvider.GetFleetItemByIdAsync(id);
            
            if (fleetItem == null)
            {
                throw new InvalidOperationException($"Véhicule avec ID {id} introuvable.");
            }

            if (fleetItem.IsInsured)
            {
                // Déjà assuré, pas besoin de mettre à jour
                return;
            }

            fleetItem.IsInsured = true;
            await _fleetProvider.UpdateFleetItemAsync(fleetItem);
        }
    }
}
