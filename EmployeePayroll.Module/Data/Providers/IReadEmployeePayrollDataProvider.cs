using EmployeePayroll.Module.Data.Models;

namespace EmployeePayroll.Module.Data.Providers
{
    public interface IReadEmployeePayrollDataProvider
    {
        Task<List<EmployeePayrollDataModel>> ReadLastMonthsPayrollAsync(long employeeId, int months);
    }
}

