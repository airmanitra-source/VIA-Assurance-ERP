using CompanyDocuments.Module.Data.Models;

namespace CompanyDocuments.Module.Data.Providers
{
    public interface ICompanyDocumentReadOnly
    {
        Task<List<CompanyDocumentDataModel>> ReadAllDocumentsAsync();
        Task<CompanyDocumentDataModel?> ReadDocumentByIdAsync(Guid streamId);
        Task<byte[]?> ReadFileContentAsync(Guid streamId);
        Task<List<CompanyDocumentDataModel>> ReadDocumentsByEntrepriseIdAsync(long entrepriseId);
    }
}
