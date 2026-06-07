using Sinister.Module.Data.Models;

namespace Sinister.Module.Data.Providers
{
    public interface ISinisterPolicyReadWriteDataProvider : ISinisterPolicyReadOnlyDataProvider
    {
        Task<long> CreatePolicyAsync(SinisterPolicyDataModel policy);
        Task UpdatePolicyAsync(SinisterPolicyDataModel policy);
        Task DeletePolicyAsync(long id);
    }
}

