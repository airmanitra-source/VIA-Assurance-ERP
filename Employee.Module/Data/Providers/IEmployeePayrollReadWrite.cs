using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IEmployeePayrollReadWrite
    {
        Task<int> CreatePayrollAsync(EmployeePayrollDataModel payroll);
        Task UpdatePayrollAsync(EmployeePayrollDataModel payroll);
        Task DeletePayrollAsync(int payrollId);
    }
}
