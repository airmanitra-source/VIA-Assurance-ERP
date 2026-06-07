using Company.Module.Data.Models;

namespace Company.Module.Data.Providers
{
    public interface IEntrepriseReadOnlyDataProvider
    {
        Task<EntrepriseDataModel?> ReadEntrepriseByIdAsync(long id);
        Task<EntrepriseDataModel?> ReadEntrepriseByEmailAsync(string email);
        Task<List<EntrepriseDataModel>> ReadAllEntreprisesAsync();
    }
}

