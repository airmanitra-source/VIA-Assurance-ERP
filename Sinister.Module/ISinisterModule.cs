using Sinister.Module.Business;

namespace Sinister.Module
{
    public interface ISinisterModule
    {
        Task<long> AddPolicyAsync(Sinister.Module.Business.SinisterPolicyBusinessModel policy);
        Task<IEnumerable<Sinister.Module.Business.SinisterPolicyBusinessModel>> GetPoliciesByEntrepriseIdAsync(long entrepriseId);
        Task<Sinister.Module.Business.SinisterPolicyBusinessModel?> GetPolicyByIdAsync(long id);
        Task UpdatePolicyAsync(Sinister.Module.Business.SinisterPolicyBusinessModel policy);
        Task RemovePolicyAsync(long id);
    }
}
