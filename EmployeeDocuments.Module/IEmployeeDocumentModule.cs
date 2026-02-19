using EmployeeDocuments.Module.Business;

namespace EmployeeDocuments.Module
{
    public interface IEmployeeDocumentModule
    {
        Task<List<EmployeeDocumentBusinessModel>> GetAllDocumentsAsync();

        Task<EmployeeDocumentBusinessModel?> GetDocumentByIdAsync(Guid streamId);

        Task<byte[]?> GetFileContentAsync(Guid streamId);

        Task<List<EmployeeDocumentBusinessModel>> GetDocumentsByEmployeeIdAsync(long employeeId);
        
        Task RemoveDocumentAsync(long employeeId, Guid streamId);

        Task<Guid> UploadAndLinkDocumentAsync(long employeeId, string fileName, byte[] fileContent, string? typeDocument);
    }
}
