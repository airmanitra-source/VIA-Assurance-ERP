using FileTable.Module.Business;
using FileTable.Module.Data.Providers;
using FileTable.Module.Data;

namespace FileTable.Module
{
    public class FileTableModule : IFileTableModule
    {
        private readonly IFileTableReadOnly _fileTableReadOnly;

        public FileTableModule(IFileTableReadOnly fileTableReadOnly)
        {
            _fileTableReadOnly = fileTableReadOnly;
        }

        public async Task<List<FileTableBusinessModel>> GetAllDocumentsAsync()
        {
            var dataModels = await _fileTableReadOnly.ReadAllDocumentsAsync();
            return dataModels.Select(ConvertToBusinessModel).ToList();
        }

        public async Task<FileTableBusinessModel?> GetDocumentByIdAsync(Guid streamId)
        {
            var dataModel = await _fileTableReadOnly.ReadDocumentByIdAsync(streamId);
            return dataModel != null ? ConvertToBusinessModel(dataModel) : null;
        }

        public async Task<byte[]?> GetFileContentAsync(Guid streamId)
        {
            return await _fileTableReadOnly.ReadFileContentAsync(streamId);
        }

        private static FileTableBusinessModel ConvertToBusinessModel(FileTableDataModel dataModel)
        {
            return new FileTableBusinessModel
            {
                StreamId = dataModel.StreamId,
                Name = dataModel.Name,
                PathLocator = dataModel.PathLocator,
                ParentPathLocator = dataModel.ParentPathLocator,
                FileType = dataModel.FileType,
                CachedFileSize = dataModel.CachedFileSize,
                CreationTime = dataModel.CreationTime,
                LastWriteTime = dataModel.LastWriteTime,
                LastAccessTime = dataModel.LastAccessTime,
                IsDirectory = dataModel.IsDirectory,
                IsOffline = dataModel.IsOffline,
                IsHidden = dataModel.IsHidden,
                IsReadonly = dataModel.IsReadonly,
                IsArchive = dataModel.IsArchive,
                IsSystem = dataModel.IsSystem,
                IsTemporary = dataModel.IsTemporary
            };
        }
    }
}
