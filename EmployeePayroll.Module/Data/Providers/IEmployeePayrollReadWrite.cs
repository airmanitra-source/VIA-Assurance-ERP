using EmployeePayroll.Module.Data.Models;

namespace EmployeePayroll.Module.Data.Providers
{
    public interface IEmployeePayrollReadWrite
    {
        Task<int> CreatePayrollAsync(EmployeePayrollDataModel payroll);
        Task DeletePayrollAsync(int payrollId);
        Task UpdatePayrollAsync(EmployeePayrollDataModel payroll);
    }
}
