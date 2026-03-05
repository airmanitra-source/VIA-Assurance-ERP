using CompanyPayroll.Module.Business;

namespace CompanyPayroll.Module
{
    public interface ICompanyPayrollModule
    {
        Task<CompanyPayrollSettingsBusinessModel> GetSettingsAsync(long enterpriseId);
        Task SetSettingsAsync(CompanyPayrollSettingsBusinessModel settings);
    }
}
