using Subscription.Module.Business;

namespace Subscription.Module
{
    public interface ISubscriptionModule
    {
        Task<long> AddSubscriptionAsync(SouscriptionBusinessModel subscription);
        Task<IEnumerable<SouscriptionBusinessModel>> GetSubscriptionsByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<SouscriptionBusinessModel>> GetSubscriptionsByEnterpriseIdAsync(long enterpriseId);
        Task SetSubscriptionAsync(SouscriptionBusinessModel subscription);
        Task RemoveSubscriptionAsync(long id);
    }
}
