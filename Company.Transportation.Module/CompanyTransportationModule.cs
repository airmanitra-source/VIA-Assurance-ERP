using Company.Transportation.Module.Business;
using Company.Transportation.Module.Data.Models;
using Company.Transportation.Module.Data.Providers;

namespace Company.Transportation.Module
{
    public class CompanyTransportationModule : ICompanyTransportationModule
    {
        private readonly IEntrepriseMerchandiseTransportationReadWrite _transportationProvider;

        public CompanyTransportationModule(IEntrepriseMerchandiseTransportationReadWrite transportationProvider)
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
            await _transportationProvider.DeleteTransportationAsync(id);
        }
    }
}
