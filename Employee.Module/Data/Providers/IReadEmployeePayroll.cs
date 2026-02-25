using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IReadEmployeePayroll
    {
        Task<List<EmployeePayrollDataModel>> ReadLastMonthsPayrollAsync(long employeeId, int months);
    }
}
