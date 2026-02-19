using CompanyDocuments.Module.Business;

namespace CompanyDocuments.Module
{
    public interface ICompanyDocumentModule
    {
        Task<List<CompanyDocumentBusinessModel>> GetAllDocumentsAsync();
        Task<CompanyDocumentBusinessModel?> GetDocumentByIdAsync(Guid streamId);
        Task<byte[]?> GetFileContentAsync(Guid streamId);
        Task<List<CompanyDocumentBusinessModel>> GetDocumentsByEntrepriseIdAsync(long entrepriseId);
        Task RemoveDocumentAsync(long entrepriseId, Guid streamId);
        Task<Guid> UploadAndLinkDocumentAsync(long entrepriseId, string fileName, byte[] fileContent, string? typeDocument, long? fleetId = null, long? warehouseId = null, long? transportationId = null);
        Task<Guid> GenerateAndLinkPolicyConfirmationAsync(long entrepriseId, string typeInsurance, PolicyPdfModel data, long? itemId = null);
        Task SignDocumentAsync(long entrepriseId, Guid streamId, string signerName, string? signatureImageBase64 = null);
    }
}
