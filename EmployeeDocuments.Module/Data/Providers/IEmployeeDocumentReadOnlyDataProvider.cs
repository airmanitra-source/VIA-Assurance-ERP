using EmployeeDocuments.Module.Data.Models;

namespace EmployeeDocuments.Module.Data.Providers
{
    public interface IEmployeeDocumentReadOnlyDataProvider
    {
        Task<List<EmployeeDocumentDataModel>> ReadAllDocumentsAsync();
        Task<List<EmployeeDocumentDataModel>> ReadDocumentsByEmployeeIdAsync(long employeeId);
        Task<EmployeeDocumentDataModel?> ReadDocumentByIdAsync(Guid streamId);
        Task<byte[]?> ReadFileContentAsync(Guid streamId);
    }
}

