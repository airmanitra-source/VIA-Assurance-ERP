namespace CompanySinisterDocument.Module.Data.Providers
{
    public interface ICompanySinisterDocumentReadWriteDataProvider
    {
        Task CreateCompanySinisterDocumentAsync(long entrepriseId, long sinisterId, Guid fileStreamId, string? typeDocument);
        Task<Guid> CreateCompanySinisterFileAsync(string fileName, byte[] fileContent);
        Task DeleteCompanySinisterDocumentAsync(long entrepriseId, long sinisterId, Guid streamId);
    }
}

