using Subscription.Module.Business;

namespace Subscription.Module
{
    public interface ISubscriptionExternalService
    {
        /// <summary>
        /// Checks if an employee has paid their monthly fee for a given month.
        /// </summary>
        /// <param name="matricule">The employee's registration number.</param>
        /// <param name="mois">The month of interest.</param>
        /// <returns>A boolean indicating success or failure of the payment check.</returns>
        Task<(bool isPaid, int MoisCotisation)> CheckPaymentStatusAsync(string matricule, int mois, int annee);
    }
}
