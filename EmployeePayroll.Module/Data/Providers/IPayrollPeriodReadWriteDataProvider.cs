using EmployeePayroll.Module.Data.Models;

namespace EmployeePayroll.Module.Data.Providers
{
    public interface IPayrollPeriodReadWriteDataProvider
    {
        Task<int> CreatePeriodAsync(PayrollPeriodDataModel period);
        Task UpdatePeriodStatusAsync(int periodId, string status);
    }
}

