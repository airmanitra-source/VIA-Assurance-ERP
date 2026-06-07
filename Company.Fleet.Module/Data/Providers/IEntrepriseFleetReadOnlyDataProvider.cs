using Company.Fleet.Module.Data.Models;

namespace Company.Fleet.Module.Data.Providers
{
    public interface IEntrepriseFleetReadOnlyDataProvider
    {
        Task<EntrepriseFleetDataModel?> GetFleetItemByIdAsync(long id);
        Task<IEnumerable<EntrepriseFleetDataModel>> GetFleetByEntrepriseIdAsync(long entrepriseId);
    }
}

