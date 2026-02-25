using Employee.Module.Business;
using Employee.Module.Data.Providers;

namespace Employee.Module
{
    public class EmployeePayrollModule : IEmployeePayrollModule
    {
        private readonly IReadEmployeePayroll _payrollReadOnly;

        public EmployeePayrollModule(IReadEmployeePayroll payrollReadOnly)
        {
            _payrollReadOnly = payrollReadOnly;
        }

        public async Task<List<EmployeePayrollBusinessModel>> GetLastMonthsPayrollAsync(long employeeId, int months = 3)
        {
            var items = await _payrollReadOnly.ReadLastMonthsPayrollAsync(employeeId, months);
            return items.Select(EmployeePayrollBusinessModel.FromDataModel).ToList();
        }
    }
}
