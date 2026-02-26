using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IPaySlipReadWrite
    {
        Task UpdatePaySlipAsync(EmployeePayrollDataModel payroll, List<PaySlipLineDataModel> lines);
    }
}
