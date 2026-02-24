using ClientApp.Models;
using Company.Transportation.Module;
using Company.Transportation.Module.Business;
using CompanyDocuments.Module;


namespace ClientApp.Controllers
{
    public class TransportationController
    {
        private readonly ICompanyTransportationModule _transportationModule;
        private readonly ICompanyDocumentModule _documentModule;

        public TransportationController(ICompanyTransportationModule transportationModule, ICompanyDocumentModule documentModule)
        {
            _transportationModule = transportationModule;
            _documentModule = documentModule;
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
        public async Task<StoreResult> Store(TransportationViewModel viewModel, long enterpriseId, string? companyRaisonSocial = null)
        {
            var result = new StoreResult();
            try
            {
                var businessModel = MapViewModelToBusinessModel(viewModel, enterpriseId);
                long transId;
                if (viewModel.Id > 0)
                {
                    await _transportationModule.SetTransportationAsync(businessModel);
                    transId = viewModel.Id;
                    result.Message = "Transportation item updated successfully!";
                }
                else
                {
                    transId = await _transportationModule.AddTransportationAsync(businessModel);
                    businessModel.Id = transId; // Ensure Id is set on business model
                    result.Message = "Transportation item added successfully!";
                }

                if (viewModel.WantsInsurance && !viewModel.IsInsured)
                {
                    // Supprimer les anciennes confirmations non signées pour éviter la redondance
                    await _documentModule.RemoveUnsignedDocumentsForAssetAsync(enterpriseId, transportationId: transId);
                    
                    // Générer et lier le document de confirmation d'assurance
                    // Le transport reste en statut "en attente" (WantsInsurance=true, IsInsured=false)
                    // IsInsured ne passera à true que lorsque le document sera SIGNÉ
                    await _documentModule.GenerateAndLinkPolicyConfirmationAsync(enterpriseId, businessModel, companyRaisonSocial ?? "Company");
                    
                    // NE PAS marquer comme assuré automatiquement
                    // Le statut "assuré" sera appliqué uniquement après signature du document
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
                ArrivalDate = b.ArrivalDate,
                DepartureDate = b.DepartureDate,
                Description = b.Description,
                Destination = b.Destination,
                EntrepriseId = b.EntrepriseId,
                Frequency = b.Frequency ?? "OneTime",
                Id = b.Id,
                InsuranceEndDate = b.InsuranceEndDate,
                InsuranceStartDate = b.InsuranceStartDate,
                IsInsured = b.IsInsured,
                Origin = b.Origin,
                PolicyNumber = b.PolicyNumber,
                Value = b.Value,
                WantsInsurance = b.WantsInsurance
            };
        }

        private EntrepriseMerchandiseTransportationBusinessModel MapViewModelToBusinessModel(TransportationViewModel vm, long enterpriseId)
        {
            return new EntrepriseMerchandiseTransportationBusinessModel
            {
                ArrivalDate = vm.ArrivalDate,
                DepartureDate = vm.DepartureDate,
                Description = vm.Description,
                Destination = vm.Destination,
                EntrepriseId = enterpriseId,
                FranchiseAmount = vm.FranchiseAmount,
                FranchisePercentage = vm.FranchisePercentage,
                FranchiseType = vm.FranchiseType,
                Frequency = vm.Frequency,
                Id = vm.Id,
                InsuranceEndDate = vm.InsuranceEndDate,
                InsuranceStartDate = vm.InsuranceStartDate,
                IsInsured = vm.IsInsured,
                Origin = vm.Origin,
                PolicyNumber = vm.PolicyNumber,
                Value = vm.Value,
                WantsInsurance = vm.WantsInsurance
            };
        }
        #endregion
    }
}
