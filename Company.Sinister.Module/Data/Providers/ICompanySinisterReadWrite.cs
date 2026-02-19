using Company.Sinister.Module.Data.Models;

namespace Company.Sinister.Module.Data.Providers
{
    public interface ICompanySinisterReadWrite : ICompanySinisterReadOnly
    {
        Task<long> AddSinisterAsync(CompanySinisterDataModel sinister);
        Task UpdateSinisterAsync(CompanySinisterDataModel sinister);
        Task DeleteSinisterAsync(long id);
    }
}
