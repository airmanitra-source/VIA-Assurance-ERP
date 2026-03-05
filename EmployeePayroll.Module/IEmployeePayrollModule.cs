using EmployeePayroll.Module.Business;

namespace EmployeePayroll.Module
{
    public interface IEmployeePayrollModule
    {
        Task<int> AddPeriodAsync(long enterpriseId, DateTime periodStart, DateTime periodEnd);
        Task<List<EmployeePayrollBusinessModel>> GetLastMonthsPayrollAsync(long employeeId, int months = 3);
        Task<List<PayrollPeriodBusinessModel>> GetPeriodsByEnterpriseAsync(long enterpriseId);
        Task SetPeriodStatusAsync(int periodId, string status);
    }
}
