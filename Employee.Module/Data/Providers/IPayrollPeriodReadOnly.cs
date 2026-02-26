using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IPayrollPeriodReadOnly
    {
        Task<List<PayrollPeriodDataModel>> ReadByEnterpriseIdAsync(long enterpriseId);
        Task<PayrollPeriodDataModel?> ReadByIdAsync(int periodId);
    }
}
