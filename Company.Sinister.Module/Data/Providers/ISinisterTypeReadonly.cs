using Company.Sinister.Module.Data.Models;

namespace Company.Sinister.Module.Data.Providers
{
    public interface ISinisterTypeReadonly
    {
        Task<List<SinisterTypeDataModel>> ReadAllAsync();
        Task<SinisterTypeDataModel?> ReadByIdAsync(long id);
        Task<SinisterTypeDataModel?> ReadByNameAsync(string typeName);
    }
}
