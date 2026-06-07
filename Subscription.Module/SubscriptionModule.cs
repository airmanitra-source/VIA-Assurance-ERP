using Employee.Module;
using Subscription.Module.Business;
using Subscription.Module.Data.Models;
using Subscription.Module.Data.Providers;

namespace Subscription.Module
{
    public class SubscriptionModule : ISubscriptionModule
    {
        private readonly ISouscriptionReadWriteDataProvider _souscriptionReadWrite;
        private readonly ISouscriptionReadOnlyDataProvider _souscriptionReadOnly;
        private readonly ISubscriptionExternalService _externalService;
        private readonly IEmployeeModule _employeeModule;

        public SubscriptionModule(
            ISouscriptionReadWriteDataProvider souscriptionReadWrite,
            ISubscriptionExternalService externalService,
            IEmployeeModule employeeModule,
            ISouscriptionReadOnlyDataProvider souscriptionReadOnly)
        {
            _souscriptionReadWrite = souscriptionReadWrite;
            _externalService = externalService;
            _employeeModule = employeeModule;
            _souscriptionReadOnly = souscriptionReadOnly;
        }

        public async Task<long> AddSubscriptionAsync(SouscriptionBusinessModel subscription)
        {
            // Fetch employee to get NumeroMatricule
            var employee = await _employeeModule.GetEmployeeByIdAsync(subscription.EmployeeId);
            
            if (employee == null || string.IsNullOrWhiteSpace(employee.NumeroMatricule))
            {
                throw new InvalidOperationException("Cannot process subscription: Employee or registration number (Matricule) not found.");
            }

            // Set default year if not provided (0 or less)
            if (subscription.AnneeCotisation <= 0)
            {
                subscription.AnneeCotisation = 2026;
            }

            // Call external API with matricule, month and year
            var (isPaid,  moisCotisation) = await _externalService.CheckPaymentStatusAsync(employee.NumeroMatricule, subscription.MoisDeCotisation, subscription.AnneeCotisation);
            
            if (!isPaid)
            {
                throw new InvalidOperationException($"Subscription rejected: Payment not confirmed for {employee.NumeroMatricule} in month {subscription.MoisDeCotisation} of year {subscription.AnneeCotisation}.");
            }

            // Check if subscription already exists for this month and year
            var alreadyExists = await _souscriptionReadOnly.ExistsAsync(subscription.EmployeeId, moisCotisation, subscription.AnneeCotisation);
            
            if (alreadyExists)
            {
                throw new InvalidOperationException($"Subscription rejected: Already paid: {employee.NumeroMatricule} in month {moisCotisation} of year {subscription.AnneeCotisation}.");
            }

            subscription.MoisDeCotisation = moisCotisation;
            var dataModel = MapToDataModel(subscription);
            return await _souscriptionReadWrite.CreateSubscriptionAsync(dataModel);
        }

        public async Task<IEnumerable<SouscriptionBusinessModel>> GetSubscriptionsByEmployeeIdAsync(int employeeId)
        {
            var subscriptions = await _souscriptionReadWrite.ReadSubscriptionsByEmployeeIdAsync(employeeId);
            return subscriptions.OrderByDescending(s => s.AnneeCotisation).ThenByDescending(s => s.MoisDeCotisation).Select(MapToBusinessModel);
        }

        public async Task<IEnumerable<SouscriptionBusinessModel>> GetSubscriptionsByEnterpriseIdAsync(long enterpriseId)
        {
            var subscriptions = await _souscriptionReadWrite.ReadSubscriptionsByEnterpriseIdAsync(enterpriseId);
            return subscriptions.Select(MapToBusinessModel);
        }

        public async Task SetSubscriptionAsync(SouscriptionBusinessModel subscription)
        {
            var dataModel = MapToDataModel(subscription);
            await _souscriptionReadWrite.UpdateSubscriptionAsync(dataModel);
        }

        public async Task RemoveSubscriptionAsync(long id)
        {
            await _souscriptionReadWrite.DeleteSubscriptionAsync(id);
        }

        private static SouscriptionDataModel MapToDataModel(SouscriptionBusinessModel businessModel)
        {
            return new SouscriptionDataModel
            {
                Id = businessModel.Id,
                EmployeeId = businessModel.EmployeeId,
                EntrepriseId = businessModel.EntrepriseId,
                MoisDeCotisation = businessModel.MoisDeCotisation,
                AnneeCotisation = businessModel.AnneeCotisation,
                MontantCotisation = businessModel.MontantCotisation
            };
        }

        private static SouscriptionBusinessModel MapToBusinessModel(SouscriptionDataModel dataModel)
        {
            return new SouscriptionBusinessModel
            {
                Id = dataModel.Id,
                EmployeeId = dataModel.EmployeeId,
                EntrepriseId = dataModel.EntrepriseId,
                MoisDeCotisation = dataModel.MoisDeCotisation,
                AnneeCotisation = dataModel.AnneeCotisation,
                MontantCotisation = dataModel.MontantCotisation
            };
        }
    }
}

