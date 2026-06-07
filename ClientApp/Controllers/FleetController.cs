using ClientApp.Models;
using Company.Fleet.Module;
using Company.Fleet.Module.Business.Models;
using CompanyDocuments.Module;


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
                    businessModel.Id = fleetId;
                    result.Message = "Fleet added successfully!";
                }

                if (viewModel.WantsInsurance && !viewModel.IsInsured)
                {
                    // Supprimer les anciennes confirmations non signées pour éviter la redondance
                    await _documentModule.RemoveUnsignedDocumentsForAssetAsync(enterpriseId, fleetId: fleetId);
                    
                    // Générer et lier le document de confirmation d'assurance
                    // Le véhicule reste en statut "en attente" (WantsInsurance=true, IsInsured=false)
                    // IsInsured ne passera à true que lorsque le document sera SIGNÉ
                    await _documentModule.GenerateAndLinkPolicyConfirmationAsync(enterpriseId, businessModel, companyRaisonSocial ?? "Company");
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
        public async Task Destroy(long id)
        {
            // Délégation simple au module - la protection est gérée dans CompanyFleetModule
            await _fleetModule.RemoveFleetItemAsync(id);
        }

        #region Mapping
        private EntrepriseFleetViewModel MapBusinessModelToViewModel(EntrepriseFleetBusinessModel b)
        {
            return new EntrepriseFleetViewModel
            {
                EntrepriseId = b.EntrepriseId,
                FranchiseAmount = b.FranchiseAmount,
                FranchisePercentage = b.FranchisePercentage,
                FranchiseType = b.FranchiseType,
                FiscalPower = b.FiscalPower,
                Id = b.Id,
                InsuranceEndDate = b.InsuranceEndDate,
                InsuranceStartDate = b.InsuranceStartDate,
                IsInsured = b.IsInsured,
                IsWorking = b.IsWorking,
                LicensePlate = b.LicensePlate,
                Make = b.Make,
                Mileage = b.Mileage,
                Model = b.Model,
                PolicyNumber = b.PolicyNumber,
                Power = b.Power,
                Type = b.Type,
                VIN = b.VIN,
                WantsInsurance = b.WantsInsurance,
                Year = b.Year
            };
        }

        private EntrepriseFleetBusinessModel MapViewModelToBusinessModel(EntrepriseFleetViewModel vm, long enterpriseId)
        {
            return new EntrepriseFleetBusinessModel
            {
                EntrepriseId = enterpriseId,
                FranchiseAmount = vm.FranchiseAmount,
                FranchisePercentage = vm.FranchisePercentage,
                FranchiseType = vm.FranchiseType,
                FiscalPower = vm.FiscalPower,
                Id = vm.Id,
                InsuranceEndDate = vm.InsuranceEndDate,
                InsuranceStartDate = vm.InsuranceStartDate,
                IsInsured = vm.IsInsured,
                IsWorking = vm.IsWorking,
                LicensePlate = vm.LicensePlate,
                Make = vm.Make,
                Mileage = vm.Mileage,
                Model = vm.Model,
                PolicyNumber = vm.PolicyNumber,
                Power = vm.Power,
                Type = vm.Type,
                VIN = vm.VIN,
                WantsInsurance = vm.WantsInsurance,
                Year = vm.Year
            };
        }
        #endregion
    }
}
