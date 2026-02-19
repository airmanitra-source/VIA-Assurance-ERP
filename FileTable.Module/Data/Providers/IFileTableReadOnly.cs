
namespace FileTable.Module.Data.Providers
{
    public interface IFileTableReadOnly
    {
        Task<List<FileTableDataModel>> ReadAllDocumentsAsync();
        Task<FileTableDataModel?> ReadDocumentByIdAsync(Guid streamId);
        Task<byte[]?> ReadFileContentAsync(Guid streamId);
    }
}