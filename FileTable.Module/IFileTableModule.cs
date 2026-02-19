using FileTable.Module.Business;

namespace FileTable.Module
{
    public interface IFileTableModule
    {
        Task<List<FileTableBusinessModel>> GetAllDocumentsAsync();

        Task<FileTableBusinessModel?> GetDocumentByIdAsync(Guid streamId);

        Task<byte[]?> GetFileContentAsync(Guid streamId);
    }
}
