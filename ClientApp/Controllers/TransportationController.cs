using ClientApp.Models;
using Company.Transportation.Module;
using Company.Transportation.Module.Business;
using CompanyDocuments.Module;
using CompanyDocuments.Module.Business;
using Microsoft.AspNetCore.Components;

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
                    result.Message = "Transportation item added successfully!";
                }

                if (viewModel.WantsInsurance && !viewModel.IsInsured)
                {
                    var policyNumber = "TRN-" + transId + "-" + DateTime.Now.Ticks.ToString().Substring(12);

                    var policyData = new PolicyPdfModel
                    {
                        PolicyNumber = policyNumber,
                        StartDate = viewModel.DepartureDate,
                        EndDate = viewModel.ArrivalDate,
                        InsuredName = companyRaisonSocial ?? "Company",
                        Address = "N/A",
                        VehicleDescription = $"Transportation: {viewModel.Description} (from {viewModel.Origin} to {viewModel.Destination})",
                        VIN = "N/A",
                        Coverages = new List<CoverageModel>
                        {
                            new CoverageModel { Description = "Cargo Insurance", Deductible = 0, Amount = viewModel.Value }
                        }
                    };

                    await _documentModule.GenerateAndLinkPolicyConfirmationAsync(enterpriseId, "Transportation", policyData, transId);

                    // Mark as insured
                    businessModel.Id = transId;
                    businessModel.IsInsured = true;
                    businessModel.PolicyNumber = policyNumber;
                    await _transportationModule.SetTransportationAsync(businessModel);
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
                ArrivalDate = b.ArrivalDate,
                DepartureDate = b.DepartureDate,
                Description = b.Description,
                Destination = b.Destination,
                EntrepriseId = b.EntrepriseId,
                Frequency = b.Frequency ?? "OneTime",
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
                Id = vm.Id,
                ArrivalDate = vm.ArrivalDate,
                DepartureDate = vm.DepartureDate,
                Description = vm.Description,
                Destination = vm.Destination,
                EntrepriseId = enterpriseId,
                Frequency = vm.Frequency,
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
