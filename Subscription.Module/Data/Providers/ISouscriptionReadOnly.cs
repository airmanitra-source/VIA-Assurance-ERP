using Subscription.Module.Data.Models;

namespace Subscription.Module.Data.Providers
{
    public interface ISouscriptionReadOnly
    {
        Task<IEnumerable<SouscriptionDataModel>> ReadSubscriptionsByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<SouscriptionDataModel>> ReadSubscriptionsByEnterpriseIdAsync(long enterpriseId);
        Task<SouscriptionDataModel?> ReadSubscriptionByIdAsync(long id);
        Task<int> ReadMaxSubscriptionMonthAsync(int employeeId);
        Task<bool> ExistsAsync(int employeeId, int month, int year); //must be changed
    }
}
