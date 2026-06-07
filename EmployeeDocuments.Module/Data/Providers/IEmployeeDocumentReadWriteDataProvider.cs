namespace EmployeeDocuments.Module.Data.Providers
{
    public interface IEmployeeDocumentReadWriteDataProvider
    {
        Task AddEmployeeDocumentAsync(long employeeId, Guid fileStreamId, string? typeDocument);
        Task<Guid> AddEmployeeFileIntoDocumentsAsync(string fileName, byte[] fileContent);
        Task DeleteEmployeeDocumentAsync(long employeeId, Guid streamId);
    }
}

