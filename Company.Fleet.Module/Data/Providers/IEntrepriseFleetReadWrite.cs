using Company.Fleet.Module.Data.Models;

namespace Company.Fleet.Module.Data.Providers
{
    public interface IEntrepriseFleetReadWrite : IEntrepriseFleetReadOnly
    {
        Task<long> AddFleetItemAsync(EntrepriseFleetDataModel fleetItem);
        Task UpdateFleetItemAsync(EntrepriseFleetDataModel fleetItem);
        Task DeleteFleetItemAsync(long id);
    }
}
