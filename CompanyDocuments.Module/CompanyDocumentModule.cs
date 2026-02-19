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

        public async Task<Guid> GenerateAndLinkPolicyConfirmationAsync(long entrepriseId, string typeInsurance, PolicyPdfModel data, long? itemId = null)
        {
            var bytes = _policyGenerator.GeneratePolicyPdf(data);
            // Append timestamp with seconds to ensure uniqueness in FileTable
            var uniqueFileName = $"Confirmation_{typeInsurance}_{data.PolicyNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            long? fleetId = typeInsurance == "Fleet" ? itemId : null;
            long? warehouseId = typeInsurance == "Warehouse" ? itemId : null;
            long? transportationId = typeInsurance == "Transportation" ? itemId : null;

            return await UploadAndLinkDocumentAsync(entrepriseId, uniqueFileName, bytes, "Confirmation Police", fleetId, warehouseId, transportationId);
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
