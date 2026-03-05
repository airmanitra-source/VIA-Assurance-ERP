using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Data.Providers
{
    public interface IPaySlipModificationRequestReadWrite
    {
        Task<int> CreateRequestAsync(PaySlipModificationRequestDataModel request);
        Task DeleteByEmployeeAndPeriodAsync(long employeeId, int periodId);
        Task UpdateRequestStatusAsync(int requestId, string status);
    }
}
