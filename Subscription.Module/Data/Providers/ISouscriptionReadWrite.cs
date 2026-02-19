using Subscription.Module.Data.Models;

namespace Subscription.Module.Data.Providers
{
    public interface ISouscriptionReadWrite : ISouscriptionReadOnly
    {
        Task<long> CreateSubscriptionAsync(SouscriptionDataModel subscription);
        Task UpdateSubscriptionAsync(SouscriptionDataModel subscription);
        Task DeleteSubscriptionAsync(long id);
    }
}
