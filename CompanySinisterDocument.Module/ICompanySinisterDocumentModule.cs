using CompanySinisterDocument.Module.Business;

namespace CompanySinisterDocument.Module
{
    public interface ICompanySinisterDocumentModule
    {
        Task<List<string>> GetAccidentPhotoDataUrlsAsync(long sinisterId);

        Task<CompanySinisterDocumentBusinessModel?> GetDocumentByIdAsync(Guid streamId);

        Task<List<CompanySinisterDocumentBusinessModel>> GetDocumentsBySinisterIdAsync(long sinisterId);

        Task<List<CompanySinisterDocumentBusinessModel>> GetAllDocumentsAsync();

        Task<byte[]?> GetFileContentAsync(Guid streamId);

        Task<Guid> AddDocumentAsync(long entrepriseId, long sinisterId, string fileName, byte[] fileContent, string? typeDocument);

        Task RemoveDocumentAsync(long entrepriseId, long sinisterId, Guid streamId);

    }
}
