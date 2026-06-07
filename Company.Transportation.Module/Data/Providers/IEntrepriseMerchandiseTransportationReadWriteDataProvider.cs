using Company.Transportation.Module.Data.Models;

namespace Company.Transportation.Module.Data.Providers
{
    public interface IEntrepriseMerchandiseTransportationReadWriteDataProvider : IEntrepriseMerchandiseTransportationReadOnlyDataProvider
    {
        Task<long> AddTransportationAsync(EntrepriseMerchandiseTransportationDataModel transportation);
        Task UpdateTransportationAsync(EntrepriseMerchandiseTransportationDataModel transportation);
        Task DeleteTransportationAsync(long id);
    }
}

