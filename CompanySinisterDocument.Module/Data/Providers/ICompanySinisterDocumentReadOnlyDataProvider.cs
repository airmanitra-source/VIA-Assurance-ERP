using CompanySinisterDocument.Module.Data.Models;

namespace CompanySinisterDocument.Module.Data.Providers
{
    public interface ICompanySinisterDocumentReadOnlyDataProvider
    {
        Task<List<CompanySinisterDocumentDataModel>> ReadAllDocumentsAsync();
        Task<CompanySinisterDocumentDataModel?> ReadDocumentByIdAsync(Guid streamId);
        Task<List<CompanySinisterDocumentDataModel>> ReadDocumentsBySinisterIdAsync(long sinisterId);
        Task<byte[]?> ReadFileContentAsync(Guid streamId);
    }
}

