namespace CompanyDocuments.Module.Data.Providers
{
    public interface ICompanyDocumentReadWriteDataProvider : ICompanyDocumentReadOnlyDataProvider
    {
        Task AddCompanyDocumentAsync(long entrepriseId, Guid fileStreamId, string? typeDocument, long? fleetId = null, long? warehouseId = null, long? transportationId = null);
        Task<Guid> AddCompanyFileIntoDocumentsAsync(string fileName, byte[] fileContent);
        Task UpdateDocumentSignatureAsync(long entrepriseId, Guid streamId, bool isSigned, DateTime? signedDate);
        Task UpdateDocumentContentAsync(Guid streamId, byte[] fileContent);
        Task DeleteCompanyDocumentAsync(long entrepriseId, Guid streamId);
    }
}

