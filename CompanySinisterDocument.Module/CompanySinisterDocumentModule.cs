using CompanySinisterDocument.Module.Business;
using CompanySinisterDocument.Module.Data.Models;
using CompanySinisterDocument.Module.Data.Providers;

namespace CompanySinisterDocument.Module
{
    public class CompanySinisterDocumentModule : ICompanySinisterDocumentModule
    {
        private readonly ICompanySinisterDocumentReadOnly _documentReadOnly;
        private readonly ICompanySinisterDocumentReadWrite _documentReadWrite;

        public CompanySinisterDocumentModule(
            ICompanySinisterDocumentReadOnly documentReadOnly,
            ICompanySinisterDocumentReadWrite documentReadWrite)
        {
            _documentReadOnly = documentReadOnly;
            _documentReadWrite = documentReadWrite;
        }

        public async Task<List<CompanySinisterDocumentBusinessModel>> GetAllDocumentsAsync()
        {
            var dataModels = await _documentReadOnly.ReadAllDocumentsAsync();
            return dataModels.Select(ConvertToBusinessModel).ToList();
        }

        public async Task<CompanySinisterDocumentBusinessModel?> GetDocumentByIdAsync(Guid streamId)
        {
            var dataModel = await _documentReadOnly.ReadDocumentByIdAsync(streamId);
            return dataModel != null ? ConvertToBusinessModel(dataModel) : null;
        }

        public async Task<List<CompanySinisterDocumentBusinessModel>> GetDocumentsBySinisterIdAsync(long sinisterId)
        {
            var dataModels = await _documentReadOnly.ReadDocumentsBySinisterIdAsync(sinisterId);
            return dataModels.Select(ConvertToBusinessModel).ToList();
        }

        public async Task<byte[]?> GetFileContentAsync(Guid streamId)
        {
            return await _documentReadOnly.ReadFileContentAsync(streamId);
        }

        public async Task<Guid> AddDocumentAsync(long entrepriseId, long sinisterId, string fileName, byte[] fileContent, string? typeDocument)
        {
            var streamId = await _documentReadWrite.CreateCompanySinisterFileAsync(fileName, fileContent);
            await _documentReadWrite.CreateCompanySinisterDocumentAsync(entrepriseId, sinisterId, streamId, typeDocument);
            return streamId;
        }

        public async Task RemoveDocumentAsync(long entrepriseId, long sinisterId, Guid streamId)
        {
            await _documentReadWrite.DeleteCompanySinisterDocumentAsync(entrepriseId, sinisterId, streamId);
        }

        private static CompanySinisterDocumentBusinessModel ConvertToBusinessModel(CompanySinisterDocumentDataModel dataModel)
        {
            return new CompanySinisterDocumentBusinessModel
            {
                CachedFileSize = dataModel.CachedFileSize,
                CreationTime = dataModel.CreationTime,
                FileType = dataModel.FileType,
                IsArchive = dataModel.IsArchive,
                IsDirectory = dataModel.IsDirectory,
                IsHidden = dataModel.IsHidden,
                IsOffline = dataModel.IsOffline,
                IsReadonly = dataModel.IsReadonly,
                IsSystem = dataModel.IsSystem,
                IsTemporary = dataModel.IsTemporary,
                LastAccessTime = dataModel.LastAccessTime,
                LastWriteTime = dataModel.LastWriteTime,
                Name = dataModel.Name,
                ParentPathLocator = dataModel.ParentPathLocator,
                PathLocator = dataModel.PathLocator,
                StreamId = dataModel.StreamId,
                TypeDocument = dataModel.TypeDocument
            };
        }
    }
}
