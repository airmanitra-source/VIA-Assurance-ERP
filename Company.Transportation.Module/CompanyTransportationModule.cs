using Company.Transportation.Module.Business;
using Company.Transportation.Module.Data.Models;
using Company.Transportation.Module.Data.Providers;

namespace Company.Transportation.Module
{
    public class CompanyTransportationModule : ICompanyTransportationModule
    {
        private readonly IEntrepriseMerchandiseTransportationReadWriteDataProvider _transportationProvider;

        public CompanyTransportationModule(IEntrepriseMerchandiseTransportationReadWriteDataProvider transportationProvider)
        {
            _transportationProvider = transportationProvider;
        }

        public async Task<EntrepriseMerchandiseTransportationBusinessModel?> GetTransportationAsync(long id)
        {
            var dataModel = await _transportationProvider.GetTransportationByIdAsync(id);
            return EntrepriseMerchandiseTransportationBusinessModel.FromDataModel(dataModel);
        }

        public async Task<IEnumerable<EntrepriseMerchandiseTransportationBusinessModel>> GetCompanyTransportationsAsync(long entrepriseId)
        {
            var dataModels = await _transportationProvider.GetTransportationsByEntrepriseIdAsync(entrepriseId);
            return dataModels.Select(EntrepriseMerchandiseTransportationBusinessModel.FromDataModel)!;
        }

        public async Task<long> AddTransportationAsync(EntrepriseMerchandiseTransportationBusinessModel transportation)
        {
            return await _transportationProvider.AddTransportationAsync(transportation.ToDataModel());
        }

        public async Task SetTransportationAsync(EntrepriseMerchandiseTransportationBusinessModel transportation)
        {
            await _transportationProvider.UpdateTransportationAsync(transportation.ToDataModel());
        }

        public async Task RemoveTransportationAsync(long id)
        {
            // Protection: vÃ©rifier si le transport est assurÃ© avant de permettre la suppression
            var transportation = await _transportationProvider.GetTransportationByIdAsync(id);
            
            if (transportation != null && transportation.IsInsured)
            {
                throw new InvalidOperationException(
                    "Impossible de supprimer ce transport car il est assurÃ©. " +
                    "La confirmation d'assurance a Ã©tÃ© signÃ©e et le transport ne peut plus Ãªtre supprimÃ©.");
            }

            await _transportationProvider.DeleteTransportationAsync(id);
        }

        public async Task MarkAsInsuredAsync(long id)
        {
            var transportation = await _transportationProvider.GetTransportationByIdAsync(id);
            
            if (transportation == null)
            {
                throw new InvalidOperationException($"Transport avec ID {id} introuvable.");
            }

            if (transportation.IsInsured)
            {
                // DÃ©jÃ  assurÃ©, pas besoin de mettre Ã  jour
                return;
            }

            transportation.IsInsured = true;
            await _transportationProvider.UpdateTransportationAsync(transportation);
        }
    }
}

