using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IPaySlipLineReadOnly
    {
        Task<List<PaySlipLineDataModel>> ReadByPayrollIdAsync(int payrollId);
        Task<List<PaySlipLineDataModel>> ReadByPeriodAndEmployeeAsync(int periodId, long employeeId);
    }
}
