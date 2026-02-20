using Company.Fleet.Module.Business;
using Company.Warehouse.Module.Business;
using Company.Transportation.Module.Business;
using CompanyDocuments.Module.Business;
using CompanyDocuments.Module.Data.Models;
using CompanyDocuments.Module.Data.Providers;

namespace CompanyDocuments.Module
{
    public class CompanyDocumentModule : ICompanyDocumentModule
    {
        private readonly ICompanyDocumentReadOnly _companyDocumentReadOnly;
        private readonly ICompanyDocumentReadWrite _companyDocumentReadWrite;
        private readonly IPolicyGenerator _policyGenerator;
        private readonly ISignatureService _signatureService;

        public CompanyDocumentModule(ICompanyDocumentReadOnly companyDocumentReadOnly, ICompanyDocumentReadWrite companyDocumentReadWrite, IPolicyGenerator policyGenerator, ISignatureService signatureService)
        {
            _companyDocumentReadOnly = companyDocumentReadOnly;
            _companyDocumentReadWrite = companyDocumentReadWrite;
            _policyGenerator = policyGenerator;
            _signatureService = signatureService;
        }

        public async Task<List<CompanyDocumentBusinessModel>> GetAllDocumentsAsync()
        {
            var dataModels = await _companyDocumentReadOnly.ReadAllDocumentsAsync();
            return dataModels.Select(ConvertToBusinessModel).ToList();
        }

        public async Task<CompanyDocumentBusinessModel?> GetDocumentByIdAsync(Guid streamId)
        {
            var dataModel = await _companyDocumentReadOnly.ReadDocumentByIdAsync(streamId);
            return dataModel != null ? ConvertToBusinessModel(dataModel) : null;
        }

        public async Task<byte[]?> GetFileContentAsync(Guid streamId)
        {
            return await _companyDocumentReadOnly.ReadFileContentAsync(streamId);
        }

        public async Task<List<CompanyDocumentBusinessModel>> GetDocumentsByEntrepriseIdAsync(long entrepriseId)
        {
            var dataModels = await _companyDocumentReadOnly.ReadDocumentsByEntrepriseIdAsync(entrepriseId);
            return dataModels.Select(ConvertToBusinessModel).ToList();
        }

        public async Task RemoveDocumentAsync(long entrepriseId, Guid streamId)
        {
            await _companyDocumentReadWrite.DeleteCompanyDocumentAsync(entrepriseId, streamId);
        }

        public async Task<Guid> UploadAndLinkDocumentAsync(long entrepriseId, string fileName, byte[] fileContent, string? typeDocument, long? fleetId = null, long? warehouseId = null, long? transportationId = null)
        {
            var streamId = await _companyDocumentReadWrite.AddCompanyFileIntoDocumentsAsync(fileName, fileContent);
            await _companyDocumentReadWrite.AddCompanyDocumentAsync(entrepriseId, streamId, typeDocument, fleetId, warehouseId, transportationId);
            return streamId;
        }

        public async Task<Guid> GenerateAndLinkPolicyConfirmationAsync(long entrepriseId, string typeInsurance, PolicyConfirmationModel data, long? itemId = null)
        {
            var bytes = _policyGenerator.GeneratePolicyPdf(data);
            // Append timestamp with seconds to ensure uniqueness in FileTable
            var uniqueFileName = $"Confirmation_{typeInsurance}_{data.PolicyNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            long? fleetId = typeInsurance == "Fleet" ? itemId : null;
            long? warehouseId = typeInsurance == "Warehouse" ? itemId : null;
            long? transportationId = typeInsurance == "Transportation" ? itemId : null;

            return await UploadAndLinkDocumentAsync(entrepriseId, uniqueFileName, bytes, "Confirmation Police", fleetId, warehouseId, transportationId);
        }

        public async Task<Guid> GenerateAndLinkPolicyConfirmationAsync(long entrepriseId, EntrepriseFleetBusinessModel fleetItem, string companyName)
        {
            var policyNumber = fleetItem.PolicyNumber;
            if (string.IsNullOrEmpty(policyNumber))
            {
                policyNumber = "FLT-" + fleetItem.Id + "-" + DateTime.Now.Ticks.ToString().Substring(12);
                fleetItem.PolicyNumber = policyNumber;
            }

            var policyData = new PolicyConfirmationModel
            {
                Title = "Confirmation Assurance auto/moto",
                PolicyNumber = policyNumber,
                StartDate = fleetItem.InsuranceStartDate ?? DateTime.Now,
                EndDate = fleetItem.InsuranceEndDate ?? DateTime.Now.AddYears(1),
                InsuredName = companyName,
                Address = "N/A",
                VehicleDescription = $"{fleetItem.Make} {fleetItem.Model} ({fleetItem.Year}) {fleetItem.Type}",
                VIN = "N/A",
                VehicleCoverages = new List<CoverageModel>
                {
                    new CoverageModel
                    {
                        Description = fleetItem.FranchiseType == "Fixed"
                            ? "Comprehensive Coverage"
                            : $"Comprehensive Coverage (Franchise {fleetItem.FranchiseType} {(fleetItem.FranchisePercentage ?? 0)}%)",
                        Deductible = fleetItem.FranchiseType == "Fixed" ? (fleetItem.FranchiseAmount ?? 0) : 0,
                        Amount = 0
                    }
                }
            };

            return await GenerateAndLinkPolicyConfirmationAsync(entrepriseId, "Fleet", policyData, fleetItem.Id);
        }

        public async Task<Guid> GenerateAndLinkPolicyConfirmationAsync(long entrepriseId, EntrepriseWarehouseBusinessModel warehouse, List<EntrepriseWarehouseMaterialBusinessModel> materials, string companyName)
        {
            var policyNumber = warehouse.PolicyNumber;
            if (string.IsNullOrEmpty(policyNumber))
            {
                policyNumber = "WH-" + warehouse.Id + "-" + DateTime.Now.Ticks.ToString().Substring(12);
                warehouse.PolicyNumber = policyNumber;
            }

            var policyData = new PolicyConfirmationModel
            {
                Title = "Confirmation Assurance Locaux",
                PolicyNumber = policyNumber,
                StartDate = warehouse.InsuranceStartDate ?? DateTime.Now,
                EndDate = warehouse.InsuranceEndDate ?? DateTime.Now.AddYears(1),
                InsuredName = companyName,
                Address = warehouse.Address,
                VehicleDescription = $"Warehouse: {warehouse.Name} ({warehouse.SizeM2} m²)",
                VIN = "N/A",
                Coverages = materials.Where(m => m.WantsInsurance).Select(m => new CoverageModel
                {
                    Description = m.Description,
                    Deductible = warehouse.FranchiseType == "Fixed" ? (warehouse.FranchiseAmount ?? 0) : 0,
                    Amount = m.ApproximateValue
                }).ToList()
            };

            return await GenerateAndLinkPolicyConfirmationAsync(entrepriseId, "Warehouse", policyData, warehouse.Id);
        }

        public async Task<Guid> GenerateAndLinkPolicyConfirmationAsync(long entrepriseId, EntrepriseMerchandiseTransportationBusinessModel transportation, string companyName)
        {
            var policyNumber = transportation.PolicyNumber;
            if (string.IsNullOrEmpty(policyNumber))
            {
                policyNumber = "TRN-" + transportation.Id + "-" + DateTime.Now.Ticks.ToString().Substring(12);
                transportation.PolicyNumber = policyNumber;
            }

            var policyData = new PolicyConfirmationModel
            {
                Title = "Confirmation Assurance voyage",
                PolicyNumber = policyNumber,
                StartDate = transportation.InsuranceStartDate ?? transportation.DepartureDate,
                EndDate = transportation.InsuranceEndDate ?? transportation.ArrivalDate,
                InsuredName = companyName,
                Address = "N/A",
                VehicleDescription = $"Transportation: {transportation.Description} (from {transportation.Origin} to {transportation.Destination})",
                VIN = "N/A",
                Coverages = new List<CoverageModel>
                {
                    new CoverageModel
                    {
                        Description = transportation.FranchiseType == "Fixed"
                            ? "Cargo Insurance"
                            : $"Cargo Insurance (Franchise {transportation.FranchiseType} {(transportation.FranchisePercentage ?? 0)}%)",
                        Deductible = transportation.FranchiseType == "Fixed" ? (transportation.FranchiseAmount ?? 0) : 0,
                        Amount = transportation.Value
                    }
                }
            };

            return await GenerateAndLinkPolicyConfirmationAsync(entrepriseId, "Transportation", policyData, transportation.Id);
        }

        public async Task SignDocumentAsync(long entrepriseId, Guid streamId, string signerName, string? signatureImageBase64 = null)
        {
            var bytes = await _companyDocumentReadOnly.ReadFileContentAsync(streamId);
            if (bytes == null) throw new Exception("Document not found.");

            var signedBytes = await _signatureService.SignPdfAsync(bytes, signerName, signatureImageBase64);
            
            await _companyDocumentReadWrite.UpdateDocumentContentAsync(streamId, signedBytes);
            await _companyDocumentReadWrite.UpdateDocumentSignatureAsync(entrepriseId, streamId, true, DateTime.Now);
        }

        private static CompanyDocumentBusinessModel ConvertToBusinessModel(CompanyDocumentDataModel dataModel)
        {
            return new CompanyDocumentBusinessModel
            {
                StreamId = dataModel.StreamId,
                Name = dataModel.Name,
                PathLocator = dataModel.PathLocator,
                ParentPathLocator = dataModel.ParentPathLocator,
                FileType = dataModel.FileType,
                CachedFileSize = dataModel.CachedFileSize,
                CreationTime = dataModel.CreationTime,
                LastWriteTime = dataModel.LastWriteTime,
                LastAccessTime = dataModel.LastAccessTime,
                IsDirectory = dataModel.IsDirectory,
                IsOffline = dataModel.IsOffline,
                IsHidden = dataModel.IsHidden,
                IsReadonly = dataModel.IsReadonly,
                IsArchive = dataModel.IsArchive,
                IsSystem = dataModel.IsSystem,
                IsTemporary = dataModel.IsTemporary,
                TypeDocument = dataModel.TypeDocument,
                IsSigned = dataModel.IsSigned,
                SignedDate = dataModel.SignedDate,
                EntrepriseFleetID = dataModel.EntrepriseFleetID,
                EntrepriseWarehouseID = dataModel.EntrepriseWarehouseID,
                EntrepriseMerchandiseTransportationID = dataModel.EntrepriseMerchandiseTransportationID
            };
        }
    }
}
