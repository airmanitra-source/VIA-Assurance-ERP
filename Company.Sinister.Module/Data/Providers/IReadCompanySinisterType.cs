using Company.Sinister.Module.Data.Models;

namespace Company.Sinister.Module.Data.Providers
{
    public interface ICompanySinisterTypeReadonly
    {
        Task<List<CompanySinisterTypeDataModel>> ReadAllAsync();
        Task<CompanySinisterTypeDataModel?> ReadByIdAsync(long id);
        Task<List<CompanySinisterTypeDataModel>> ReadBySinisterIdAsync(long sinisterId);
        Task<List<SinisterTypeDataModel>> ReadSinisterTypesBySinisterIdAsync(long sinisterId);
    }
}
