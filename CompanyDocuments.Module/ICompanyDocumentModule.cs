using CompanyDocuments.Module.Business;
using Company.Fleet.Module.Business;
using Company.Warehouse.Module.Business;
using Company.Transportation.Module.Business;

namespace CompanyDocuments.Module
{
    public interface ICompanyDocumentModule
    {
        Task<List<CompanyDocumentBusinessModel>> GetAllDocumentsAsync();
        Task<CompanyDocumentBusinessModel?> GetDocumentByIdAsync(Guid streamId);
        Task<byte[]?> GetFileContentAsync(Guid streamId);
        Task<List<CompanyDocumentBusinessModel>> GetDocumentsByEntrepriseIdAsync(long entrepriseId);
        Task RemoveDocumentAsync(long entrepriseId, Guid streamId);
        Task RemoveUnsignedDocumentsForAssetAsync(long entrepriseId, long? fleetId = null, long? warehouseId = null, long? transportationId = null);
        Task<Guid> UploadAndLinkDocumentAsync(long entrepriseId, string fileName, byte[] fileContent, string? typeDocument, long? fleetId = null, long? warehouseId = null, long? transportationId = null);
        Task<Guid> GenerateAndLinkPolicyConfirmationAsync(long entrepriseId, string typeInsurance, PolicyConfirmationModel data, long? itemId = null);
        Task<Guid> GenerateAndLinkPolicyConfirmationAsync(long entrepriseId, EntrepriseFleetBusinessModel fleetItem, string companyName);
        Task<Guid> GenerateAndLinkPolicyConfirmationAsync(long entrepriseId, EntrepriseWarehouseBusinessModel warehouse, List<EntrepriseWarehouseMaterialBusinessModel> materials, string companyName);
        Task<Guid> GenerateAndLinkPolicyConfirmationAsync(long entrepriseId, EntrepriseMerchandiseTransportationBusinessModel transportation, string companyName);
        Task SignDocumentAsync(long entrepriseId, Guid streamId, string signerName, string? signatureImageBase64 = null);
    }
}
