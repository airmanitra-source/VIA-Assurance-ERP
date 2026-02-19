using ClientApp.Models;
using Company.Fleet.Module;
using Company.Fleet.Module.Business;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Controllers
{
    public class FleetController
    {
        private readonly ICompanyFleetModule _fleetModule;

        public FleetController(ICompanyFleetModule fleetModule)
        {
            _fleetModule = fleetModule;
        }

        /// <summary>
        /// REST: Index - Get all fleet items for an enterprise
        /// </summary>
        public async Task<List<EntrepriseFleetViewModel>> Index(long enterpriseId)
        {
            var fleets = await _fleetModule.GetCompanyFleetAsync(enterpriseId);
            return fleets.Select(MapBusinessModelToViewModel).ToList();
        }

        /// <summary>
        /// REST: Show - Get a single fleet item
        /// </summary>
        public async Task<EntrepriseFleetViewModel?> Show(long id, long enterpriseId)
        {
            var fleets = await _fleetModule.GetCompanyFleetAsync(enterpriseId);
            var fleet = fleets.FirstOrDefault(f => f.Id == id);
            return fleet != null ? MapBusinessModelToViewModel(fleet) : null;
        }

        /// <summary>
        /// REST: Store - Create or Update fleet item
        /// </summary>
        public async Task<StoreResult> Store(EntrepriseFleetViewModel viewModel, long enterpriseId)
        {
            var result = new StoreResult();
            try
            {
                var businessModel = MapViewModelToBusinessModel(viewModel, enterpriseId);
                if (viewModel.Id > 0)
                {
                    await _fleetModule.SetFleetItemAsync(businessModel);
                    result.Message = "Fleet updated successfully!";
                }
                else
                {
                    await _fleetModule.AddFleetItemAsync(businessModel);
                    result.Message = "Fleet added successfully!";
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Failed to save fleet: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// REST: Destroy - Delete a fleet item
        /// </summary>
        public async Task<bool> Destroy(long id)
        {
            try
            {
                await _fleetModule.RemoveFleetItemAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Mapping
        private EntrepriseFleetViewModel MapBusinessModelToViewModel(EntrepriseFleetBusinessModel business)
        {
            return new EntrepriseFleetViewModel
            {
                Id = business.Id,
                EntrepriseId = business.EntrepriseId,
                IsInsured = business.IsInsured,
                IsWorking = business.IsWorking,
                Make = business.Make,
                Mileage = business.Mileage,
                Model = business.Model,
                Type = business.Type,
                WantsInsurance = business.WantsInsurance,
                Year = business.Year,
                PolicyNumber = business.PolicyNumber
            };
        }

        private EntrepriseFleetBusinessModel MapViewModelToBusinessModel(EntrepriseFleetViewModel vm, long enterpriseId)
        {
            return new EntrepriseFleetBusinessModel
            {
                Id = vm.Id,
                EntrepriseId = enterpriseId,
                IsInsured = vm.IsInsured,
                IsWorking = vm.IsWorking,
                Make = vm.Make,
                Mileage = vm.Mileage,
                Model = vm.Model,
                Type = vm.Type,
                WantsInsurance = vm.WantsInsurance,
                Year = vm.Year,
                PolicyNumber = vm.PolicyNumber
            };
        }
        #endregion
    }
}
