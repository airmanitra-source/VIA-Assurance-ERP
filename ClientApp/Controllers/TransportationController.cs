using ClientApp.Models;
using Company.Transportation.Module;
using Company.Transportation.Module.Business;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Controllers
{
    public class TransportationController
    {
        private readonly ICompanyTransportationModule _transportationModule;

        public TransportationController(ICompanyTransportationModule transportationModule)
        {
            _transportationModule = transportationModule;
        }

        /// <summary>
        /// REST: Index
        /// </summary>
        public async Task<List<TransportationViewModel>> Index(long enterpriseId)
        {
            var items = await _transportationModule.GetCompanyTransportationsAsync(enterpriseId);
            return items.Select(MapBusinessModelToViewModel).ToList();
        }

        /// <summary>
        /// REST: Show
        /// </summary>
        public async Task<TransportationViewModel?> Show(long id, long enterpriseId)
        {
            var transportations = await _transportationModule.GetCompanyTransportationsAsync(enterpriseId);
            var trans = transportations.FirstOrDefault(t => t.Id == id);
            return trans != null ? MapBusinessModelToViewModel(trans) : null;
        }

        /// <summary>
        /// REST: Store
        /// </summary>
        public async Task<StoreResult> Store(TransportationViewModel viewModel, long enterpriseId)
        {
            var result = new StoreResult();
            try
            {
                var businessModel = MapViewModelToBusinessModel(viewModel, enterpriseId);
                if (viewModel.Id > 0)
                {
                    await _transportationModule.SetTransportationAsync(businessModel);
                    result.Message = "Transportation item updated successfully!";
                }
                else
                {
                    await _transportationModule.AddTransportationAsync(businessModel);
                    result.Message = "Transportation item added successfully!";
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Failed to save transportation item: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// REST: Destroy
        /// </summary>
        public async Task<bool> Destroy(long id)
        {
            try
            {
                await _transportationModule.RemoveTransportationAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Mapping
        private TransportationViewModel MapBusinessModelToViewModel(EntrepriseMerchandiseTransportationBusinessModel b)
        {
            return new TransportationViewModel
            {
                Id = b.Id,
                EntrepriseId = b.EntrepriseId,
                ArrivalDate = b.ArrivalDate,
                DepartureDate = b.DepartureDate,
                Description = b.Description,
                Destination = b.Destination,
                Frequency = b.Frequency ?? "OneTime",
                IsInsured = b.IsInsured,
                Origin = b.Origin,
                Value = b.Value,
                WantsInsurance = b.WantsInsurance
            };
        }

        private EntrepriseMerchandiseTransportationBusinessModel MapViewModelToBusinessModel(TransportationViewModel vm, long enterpriseId)
        {
            return new EntrepriseMerchandiseTransportationBusinessModel
            {
                Id = vm.Id,
                EntrepriseId = enterpriseId,
                ArrivalDate = vm.ArrivalDate,
                DepartureDate = vm.DepartureDate,
                Description = vm.Description,
                Destination = vm.Destination,
                Frequency = vm.Frequency,
                IsInsured = vm.IsInsured,
                Origin = vm.Origin,
                Value = vm.Value,
                WantsInsurance = vm.WantsInsurance
            };
        }
        #endregion
    }
}
