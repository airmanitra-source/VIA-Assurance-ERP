using CompanyPayroll.Module.Data.Models;

namespace CompanyPayroll.Module.Data.Providers
{
    public interface ICompanyPayrollSettingsReadWriteDataProvider
    {
        Task<int> CreateSettingsAsync(CompanyPayrollSettingsDataModel settings);
        Task UpdateSettingsAsync(CompanyPayrollSettingsDataModel settings);
    }
}

