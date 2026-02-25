using Employee.Module.Business;

namespace Employee.Module
{
    public interface IEmployeePayrollModule
    {
        Task<List<EmployeePayrollBusinessModel>> GetLastMonthsPayrollAsync(long employeeId, int months = 3);
    }
}
