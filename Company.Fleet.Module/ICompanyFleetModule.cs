using Company.Fleet.Module.Business;

namespace Company.Fleet.Module
{
    public interface ICompanyFleetModule
    {
        Task<EntrepriseFleetBusinessModel?> GetFleetItemAsync(long id);
        Task<List<EntrepriseFleetBusinessModel>> GetCompanyFleetAsync(long entrepriseId);
        Task<long> AddFleetItemAsync(EntrepriseFleetBusinessModel fleetItem);
        Task SetFleetItemAsync(EntrepriseFleetBusinessModel fleetItem);
        Task RemoveFleetItemAsync(long id);
        Task MarkAsInsuredAsync(long id);
    }
}
