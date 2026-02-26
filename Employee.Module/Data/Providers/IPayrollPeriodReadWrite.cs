using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IPayrollPeriodReadWrite
    {
        Task<int> CreatePeriodAsync(PayrollPeriodDataModel period);
        Task UpdatePeriodStatusAsync(int periodId, string status);
    }
}
