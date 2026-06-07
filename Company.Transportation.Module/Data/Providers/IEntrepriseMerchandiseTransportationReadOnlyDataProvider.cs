using Company.Transportation.Module.Data.Models;

namespace Company.Transportation.Module.Data.Providers
{
    public interface IEntrepriseMerchandiseTransportationReadOnlyDataProvider
    {
        Task<EntrepriseMerchandiseTransportationDataModel?> GetTransportationByIdAsync(long id);
        Task<IEnumerable<EntrepriseMerchandiseTransportationDataModel>> GetTransportationsByEntrepriseIdAsync(long entrepriseId);
    }
}

