using CompanyPayroll.Module.Data.Models;

namespace CompanyPayroll.Module.Data.Providers
{
    public interface ICompanyPayrollSettingsReadOnlyDataProvider
    {
        Task<CompanyPayrollSettingsDataModel?> ReadByEnterpriseIdAsync(long enterpriseId);
    }
}

