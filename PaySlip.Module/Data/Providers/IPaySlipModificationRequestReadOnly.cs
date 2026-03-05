using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Data.Providers
{
    public interface IPaySlipModificationRequestReadOnly
    {
        Task<PaySlipModificationRequestDataModel?> ReadPendingByEmployeeAndPeriodAsync(long employeeId, int periodId);
        Task<List<PaySlipModificationRequestDataModel>> ReadPendingByPeriodAsync(int periodId);
    }
}
