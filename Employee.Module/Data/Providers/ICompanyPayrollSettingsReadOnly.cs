using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface ICompanyPayrollSettingsReadOnly
    {
        Task<CompanyPayrollSettingsDataModel?> ReadByEnterpriseIdAsync(long enterpriseId);
    }
}
