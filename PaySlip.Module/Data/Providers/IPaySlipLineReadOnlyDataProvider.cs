using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Data.Providers
{
    public interface IPaySlipLineReadOnlyDataProvider
    {
        Task<List<PaySlipLineDataModel>> ReadByPayrollIdAsync(int payrollId);
        Task<List<PaySlipLineDataModel>> ReadByPeriodAndEmployeeAsync(int periodId, long employeeId);
    }
}

