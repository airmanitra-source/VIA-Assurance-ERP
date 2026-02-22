using ClientApp.Models;
using Subscription.Module;
using Subscription.Module.Business;
using Microsoft.AspNetCore.Authorization;

namespace ClientApp.Controllers
{
    [Authorize(Roles = "developer")]
    public class SubscriptionController
    {
        private readonly ISubscriptionModule _subscriptionModule;

        public SubscriptionController(ISubscriptionModule subscriptionModule)
        {
            _subscriptionModule = subscriptionModule;
        }

        /// <summary>
        /// REST: Index - Get all subscriptions for an employee
        /// </summary>
        public async Task<List<SubscriptionViewModel>> Index(long employeeId)
        {
            var subscriptions = await _subscriptionModule.GetSubscriptionsByEmployeeIdAsync((int)employeeId);
            return subscriptions.Select(MapBusinessModelToViewModel).ToList();
        }

        /// <summary>
        /// REST: Show - Get a single subscription (if needed)
        /// </summary>
        public async Task<SubscriptionViewModel?> Show(long id)
        {
            // SubscriptionModule doesn't have a GetById, but we can list all for enterprise/employee and filter
            // Or if we had a dedicated provider as per standard, we'd use it.
            // For now, let's assume we don't need Show for individual subscriptions.
            return null; 
        }

        /// <summary>
        /// REST: Store - Add or update a subscription
        /// </summary>
        public async Task<StoreResult> Store(SubscriptionViewModel viewModel)
        {
            var result = new StoreResult();
            try
            {
                var businessModel = MapViewModelToBusinessModel(viewModel);
                if (viewModel.Id > 0)
                {
                    await _subscriptionModule.SetSubscriptionAsync(businessModel);
                    result.Message = "Subscription updated successfully!";
                }
                else
                {
                    await _subscriptionModule.AddSubscriptionAsync(businessModel);
                    result.Message = "Subscription added successfully!";
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Failed to save subscription: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// REST: Destroy - Remove a subscription
        /// </summary>
        public async Task<bool> Destroy(long id)
        {
            try
            {
                await _subscriptionModule.RemoveSubscriptionAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Mapping
        private SubscriptionViewModel MapBusinessModelToViewModel(SouscriptionBusinessModel business)
        {
            return new SubscriptionViewModel
            {
                Id = business.Id,
                AnneeCotisation = business.AnneeCotisation,
                EmployeeId = business.EmployeeId,
                EntrepriseId = business.EntrepriseId,
                MoisDeCotisation = business.MoisDeCotisation,
                MontantCotisation = business.MontantCotisation
            };
        }

        private SouscriptionBusinessModel MapViewModelToBusinessModel(SubscriptionViewModel vm)
        {
            return new SouscriptionBusinessModel
            {
                Id = vm.Id,
                AnneeCotisation = vm.AnneeCotisation,
                EmployeeId = (int)vm.EmployeeId,
                EntrepriseId = vm.EntrepriseId,
                MoisDeCotisation = vm.MoisDeCotisation,
                MontantCotisation = vm.MontantCotisation
            };
        }
        #endregion
    }
}
