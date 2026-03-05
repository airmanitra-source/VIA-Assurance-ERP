using EmployeePayroll.Module.Data.Models;

namespace EmployeePayroll.Module.Data.Providers
{
    public interface IPayrollPeriodReadOnly
    {
        Task<List<PayrollPeriodDataModel>> ReadByEnterpriseIdAsync(long enterpriseId);
        Task<PayrollPeriodDataModel?> ReadByIdAsync(int periodId);
    }
}
