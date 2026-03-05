using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Data.Providers
{
    public interface IPaySlipSecondEntryReadWrite
    {
        Task<int> CreateSecondEntryAsync(PaySlipSecondEntryDataModel entry);
        Task DeleteByEmployeeAndPeriodAsync(long employeeId, int periodId);
    }
}
