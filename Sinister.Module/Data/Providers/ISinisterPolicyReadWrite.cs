using Sinister.Module.Data.Models;

namespace Sinister.Module.Data.Providers
{
    public interface ISinisterPolicyReadWrite : ISinisterPolicyReadOnly
    {
        Task<long> CreatePolicyAsync(SinisterPolicyDataModel policy);
        Task UpdatePolicyAsync(SinisterPolicyDataModel policy);
        Task DeletePolicyAsync(long id);
    }
}
