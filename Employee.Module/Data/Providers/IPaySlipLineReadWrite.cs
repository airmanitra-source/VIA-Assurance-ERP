using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IPaySlipLineReadWrite
    {
        Task CreateLinesAsync(List<PaySlipLineDataModel> lines);
        Task DeleteByPayrollIdAsync(int payrollId);
    }
}
