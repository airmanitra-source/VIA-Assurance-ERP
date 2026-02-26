using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IPaySlipModificationRequestReadWrite
    {
        Task<int> CreateRequestAsync(PaySlipModificationRequestDataModel request);
        Task UpdateRequestStatusAsync(int requestId, string status);
    }
}
