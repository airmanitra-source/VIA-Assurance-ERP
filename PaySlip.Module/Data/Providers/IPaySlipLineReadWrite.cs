using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Data.Providers
{
    public interface IPaySlipLineReadWrite
    {
        Task CreateLinesAsync(List<PaySlipLineDataModel> lines);
        Task DeleteByPayrollIdAsync(int payrollId);
    }
}
