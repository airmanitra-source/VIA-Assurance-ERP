using Company.Transportation.Module.Business;

namespace Company.Transportation.Module
{
    public interface ICompanyTransportationModule
    {
        Task<EntrepriseMerchandiseTransportationBusinessModel?> GetTransportationAsync(long id);
        Task<IEnumerable<EntrepriseMerchandiseTransportationBusinessModel>> GetCompanyTransportationsAsync(long entrepriseId);
        Task<long> AddTransportationAsync(EntrepriseMerchandiseTransportationBusinessModel transportation);
        Task SetTransportationAsync(EntrepriseMerchandiseTransportationBusinessModel transportation);
        Task RemoveTransportationAsync(long id);
    }
}
