using CompanyPayroll.Module.Data.Models;

namespace CompanyPayroll.Module.Data.Providers
{
    public interface ICompanyPayrollSettingsReadWrite
    {
        Task<int> CreateSettingsAsync(CompanyPayrollSettingsDataModel settings);
        Task UpdateSettingsAsync(CompanyPayrollSettingsDataModel settings);
    }
}
