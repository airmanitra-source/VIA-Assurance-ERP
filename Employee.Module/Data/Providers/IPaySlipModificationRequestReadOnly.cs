using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IPaySlipModificationRequestReadOnly
    {
        Task<PaySlipModificationRequestDataModel?> ReadPendingByEmployeeAndPeriodAsync(long employeeId, int periodId);
        Task<List<PaySlipModificationRequestDataModel>> ReadPendingByPeriodAsync(int periodId);
    }
}
