using Company.Fleet.Module.Data.Models;

namespace Company.Fleet.Module.Data.Providers
{
    public interface IEntrepriseFleetReadWriteDataProvider : IEntrepriseFleetReadOnlyDataProvider
    {
        Task<long> AddFleetItemAsync(EntrepriseFleetDataModel fleetItem);
        Task UpdateFleetItemAsync(EntrepriseFleetDataModel fleetItem);
        Task DeleteFleetItemAsync(long id);
    }
}

