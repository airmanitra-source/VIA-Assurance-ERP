using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Data.Providers
{
    public interface IPaySlipModificationRequestReadOnlyDataProvider
    {
        Task<PaySlipModificationRequestDataModel?> ReadPendingByEmployeeAndPeriodAsync(long employeeId, int periodId);
        Task<List<PaySlipModificationRequestDataModel>> ReadPendingByPeriodAsync(int periodId);
    }
}

