using ClientApp.Models;
using Company.Fleet.Module;
using Company.Fleet.Module.Business;
using CompanyDocuments.Module;
using CompanyDocuments.Module.Business;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Controllers
{
    public class FleetController
    {
        private readonly ICompanyFleetModule _fleetModule;
        private readonly ICompanyDocumentModule _documentModule;

        public FleetController(ICompanyFleetModule fleetModule, ICompanyDocumentModule documentModule)
        {
            _fleetModule = fleetModule;
            _documentModule = documentModule;
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
        public async Task<StoreResult> Store(EntrepriseFleetViewModel viewModel, long enterpriseId, string? companyRaisonSocial = null)
        {
            var result = new StoreResult();
            try
            {
                var businessModel = MapViewModelToBusinessModel(viewModel, enterpriseId);
                long fleetId;
                if (viewModel.Id > 0)
                {
                    await _fleetModule.SetFleetItemAsync(businessModel);
                    fleetId = viewModel.Id;
                    result.Message = "Fleet updated successfully!";
                }
                else
                {
                    fleetId = await _fleetModule.AddFleetItemAsync(businessModel);
                    result.Message = "Fleet added successfully!";
                }

                if (viewModel.WantsInsurance && !viewModel.IsInsured)
                {
                    var policyNumber = "FLT-" + fleetId + "-" + DateTime.Now.Ticks.ToString().Substring(12);

                    var policyData = new PolicyPdfModel
                    {
                        PolicyNumber = policyNumber,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddYears(1),
                        InsuredName = companyRaisonSocial ?? "Company",
                        Address = "N/A",
                        VehicleDescription = $"{viewModel.Make} {viewModel.Model} ({viewModel.Year}) {viewModel.Type}",
                        VIN = "N/A",
                        VehicleCoverages = new List<CoverageModel>
                        {
                            new CoverageModel { Description = "Comprehensive Coverage", Deductible = 500, Amount = 0 }
                        }
                    };

                    await _documentModule.GenerateAndLinkPolicyConfirmationAsync(enterpriseId, "Fleet", policyData, fleetId);

                    // Mark as insured
                    businessModel.Id = fleetId;
                    businessModel.IsInsured = true;
                    businessModel.PolicyNumber = policyNumber;
                    await _fleetModule.SetFleetItemAsync(businessModel);
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
        private EntrepriseFleetViewModel MapBusinessModelToViewModel(EntrepriseFleetBusinessModel b)
        {
            return new EntrepriseFleetViewModel
            {
                Id = b.Id,
                EntrepriseId = b.EntrepriseId,
                IsInsured = b.IsInsured,
                IsWorking = b.IsWorking,
                Make = b.Make,
                Mileage = b.Mileage,
                Model = b.Model,
                PolicyNumber = b.PolicyNumber,
                Type = b.Type,
                WantsInsurance = b.WantsInsurance,
                Year = b.Year
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
                PolicyNumber = vm.PolicyNumber,
                Type = vm.Type,
                WantsInsurance = vm.WantsInsurance,
                Year = vm.Year
            };
        }
        #endregion
    }
}
