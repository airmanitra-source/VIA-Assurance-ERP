using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Data.Providers
{
    public interface IPaySlipSecondEntryReadOnlyDataProvider
    {
        Task<PaySlipSecondEntryDataModel?> ReadByEmployeeAndPeriodAsync(long employeeId, int periodId);
    }
}

