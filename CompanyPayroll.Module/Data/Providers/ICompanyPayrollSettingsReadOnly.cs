using CompanyPayroll.Module.Data.Models;

namespace CompanyPayroll.Module.Data.Providers
{
    public interface ICompanyPayrollSettingsReadOnly
    {
        Task<CompanyPayrollSettingsDataModel?> ReadByEnterpriseIdAsync(long enterpriseId);
    }
}
