using Company.Warehouse.Module.Business;
using Company.Transportation.Module.Business;
using CompanyDocuments.Module.Business;
using CompanyDocuments.Module.Data.Models;
using CompanyDocuments.Module.Data.Providers;
using Company.Fleet.Module.Business.Models;

namespace CompanyDocuments.Module
{
    public class CompanyDocumentModule : ICompanyDocumentModule
    {
        private readonly ICompanyDocumentReadOnlyDataProvider _companyDocumentReadOnly;
        private readonly ICompanyDocumentReadWriteDataProvider _companyDocumentReadWrite;
        private readonly IPolicyGenerator _policyGenerator;
        private readonly ISignatureService _signatureService;

        public CompanyDocumentModule(ICompanyDocumentReadOnlyDataProvider companyDocumentReadOnly, ICompanyDocumentReadWriteDataProvider companyDocumentReadWrite, IPolicyGenerator policyGenerator, ISignatureService signatureService)
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

        public async Task RemoveUnsignedDocumentsForAssetAsync(long entrepriseId, long? fleetId = null, long? warehouseId = null, long? transportationId = null)
        {
            // RÃ©cupÃ©rer tous les documents de l'entreprise
            var allDocs = await _companyDocumentReadOnly.ReadDocumentsByEntrepriseIdAsync(entrepriseId);
            
            // Filtrer pour trouver les documents non signÃ©s liÃ©s Ã  l'actif spÃ©cifiÃ©
            var unsignedDocsToDelete = allDocs.Where(doc => 
                !doc.IsSigned && // Document non signÃ©
                doc.TypeDocument == "Confirmation Police" && // Type confirmation d'assurance
                (
                    (fleetId.HasValue && doc.EntrepriseFleetID == fleetId.Value) ||
                    (warehouseId.HasValue && doc.EntrepriseWarehouseID == warehouseId.Value) ||
                    (transportationId.HasValue && doc.EntrepriseMerchandiseTransportationID == transportationId.Value)
                )
            ).ToList();

            // Supprimer chaque document trouvÃ©
            foreach (var doc in unsignedDocsToDelete)
            {
                await _companyDocumentReadWrite.DeleteCompanyDocumentAsync(entrepriseId, doc.StreamId);
            }
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

            var (fleetDeductible, fleetDeductibleDisplay) = CalculateDeductible(fleetItem.FranchiseType, fleetItem.FranchiseAmount, fleetItem.FranchisePercentage, null);

            var policyData = new PolicyConfirmationModel
            {
                Title = "Confirmation Assurance auto/moto",
                PolicyNumber = policyNumber,
                StartDate = fleetItem.InsuranceStartDate ?? DateTime.Now,
                EndDate = fleetItem.InsuranceEndDate ?? DateTime.Now.AddYears(1),
                InsuredName = companyName,
                Address = "N/A",
                VehicleDescription = $"{fleetItem.Make} {fleetItem.Model} ({fleetItem.Year}) {fleetItem.Type}",
                VIN = !string.IsNullOrEmpty(fleetItem.VIN) ? fleetItem.VIN : "Non renseignÃ©",
                LicensePlate = !string.IsNullOrEmpty(fleetItem.LicensePlate) ? fleetItem.LicensePlate : "Non renseignÃ©e",
                VehicleCoverages = new List<CoverageModel>
                {
                    // ResponsabilitÃ© civile obligatoire
                    new CoverageModel
                    {
                        Description = "ResponsabilitÃ© Civile",
                        Deductible = 0,
                        Amount = 10_000_000 // 10 000 000 Ariary
                    },
                    // Garantie principale avec franchise
                    new CoverageModel
                    {
                        Description = fleetItem.FranchiseType == "Fixed"
                            ? "Couverture Tous Risques - Franchise fixe"
                            : $"Couverture Tous Risques - Franchise {fleetItem.FranchiseType} ({fleetItem.FranchisePercentage ?? 0}%)",
                        Deductible = fleetDeductible,
                        DeductibleDisplay = fleetDeductibleDisplay,
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
                VehicleDescription = $"Warehouse: {warehouse.Name} ({warehouse.SizeM2} mÂ²)",
                VIN = "N/A",
                Coverages = materials.Where(m => m.WantsInsurance).Select(m =>
                {
                    var (deductible, deductibleDisplay) = CalculateDeductible(warehouse.FranchiseType, warehouse.FranchiseAmount, warehouse.FranchisePercentage, m.ApproximateValue);
                    return new CoverageModel
                    {
                        Description = m.Description,
                        Deductible = deductible,
                        DeductibleDisplay = deductibleDisplay,
                        Amount = m.ApproximateValue
                    };
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

            var (transportDeductible, transportDeductibleDisplay) = CalculateDeductible(transportation.FranchiseType, transportation.FranchiseAmount, transportation.FranchisePercentage, transportation.Value);

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
                        Deductible = transportDeductible,
                        DeductibleDisplay = transportDeductibleDisplay,
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

        private static (decimal Deductible, string? DeductibleDisplay) CalculateDeductible(string? franchiseType, decimal? franchiseAmount, decimal? franchisePercentage, decimal? insuredAmount)
        {
            if (string.Equals(franchiseType, "Fixed", StringComparison.OrdinalIgnoreCase))
            {
                return (franchiseAmount ?? 0, null);
            }

            var percentage = franchisePercentage ?? 0;
            if (insuredAmount.HasValue && insuredAmount.Value > 0 && percentage > 0)
            {
                var deductible = Math.Round(insuredAmount.Value * percentage / 100m, 0);
                return (deductible, null);
            }

            return (0, percentage > 0 ? $"{percentage:0.##} %" : "-");
        }
    }
}

