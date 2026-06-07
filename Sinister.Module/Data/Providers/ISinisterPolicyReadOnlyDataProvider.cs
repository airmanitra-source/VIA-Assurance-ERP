using Sinister.Module.Data.Models;

namespace Sinister.Module.Data.Providers
{
    public interface ISinisterPolicyReadOnlyDataProvider
    {
        Task<IEnumerable<SinisterPolicyDataModel>> ReadPoliciesByEntrepriseIdAsync(long entrepriseId);
        Task<SinisterPolicyDataModel?> ReadPolicyByIdAsync(long id);
        Task<SinisterPolicyDataModel?> ReadPolicyByPolicyNumberAsync(string policyNumber);
    }
}

