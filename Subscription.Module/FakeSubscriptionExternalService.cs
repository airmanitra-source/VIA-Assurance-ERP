using Subscription.Module.Business;

namespace Subscription.Module
{
    public class FakeSubscriptionExternalService : ISubscriptionExternalService
    {
        public async Task<(bool isPaid, int MoisCotisation)> CheckPaymentStatusAsync(string matricule, int mois, int annee)
        {
            // Simulate API delay
            await Task.Delay(500);

            // Simulate logic: reject if matricule is empty or certain months are unpaid
            if (string.IsNullOrWhiteSpace(matricule))
            {
                return (false, 0);
            }

            var randomMonthSelector = new Random();
            return (true, randomMonthSelector.Next(1, 13));
        }
    }
}
