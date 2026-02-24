using Company.Sinister.Module.Data.Models;

namespace Company.Sinister.Module.Data.Providers
{
    public interface ICompanySinisterTypeReadWrite : ICompanySinisterTypeReadonly
    {
        Task CreateSinisterTypeAsync(long sinisterId, long sinisterTypeId);
        Task CreateSinisterTypesAsync(long sinisterId, IEnumerable<long> sinisterTypeIds);
        Task DeleteSinisterTypesAsync(long sinisterId);
    }
}
