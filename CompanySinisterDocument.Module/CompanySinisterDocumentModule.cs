using CompanySinisterDocument.Module.Business;
using CompanySinisterDocument.Module.Data.Models;
using CompanySinisterDocument.Module.Data.Providers;

namespace CompanySinisterDocument.Module
{
    public class CompanySinisterDocumentModule : ICompanySinisterDocumentModule
    {
        private readonly ICompanySinisterDocumentReadOnlyDataProvider _documentReadOnly;
        private readonly ICompanySinisterDocumentReadWriteDataProvider _documentReadWrite;

        public CompanySinisterDocumentModule(
            ICompanySinisterDocumentReadOnlyDataProvider documentReadOnly,
            ICompanySinisterDocumentReadWriteDataProvider documentReadWrite)
        {
            _documentReadOnly = documentReadOnly;
            _documentReadWrite = documentReadWrite;
        }

        public async Task<List<CompanySinisterDocumentBusinessModel>> GetAllDocumentsAsync()
        {
            var dataModels = await _documentReadOnly.ReadAllDocumentsAsync();
            return dataModels.Select(ConvertToBusinessModel).ToList();
        }

        public async Task<string?> GetAccidentPhotoDataUrlAsync(long sinisterId)
        {
            var documents = await GetDocumentsBySinisterIdAsync(sinisterId);
            var photo = documents.FirstOrDefault(d => string.Equals(d.TypeDocument, "Photo", StringComparison.OrdinalIgnoreCase));
            if (photo == null)
            {
                return null;
            }

            var extension = (photo.FileType ?? Path.GetExtension(photo.Name))?.TrimStart('.').ToLowerInvariant() ?? string.Empty;
            var mimeType = GetImageMimeType(extension);
            if (mimeType == null)
            {
                return null;
            }

            var content = await GetFileContentAsync(photo.StreamId);
            if (content == null || content.Length == 0)
            {
                return null;
            }

            var base64 = Convert.ToBase64String(content);
            return $"data:{mimeType};base64,{base64}";
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

        private static string? GetImageMimeType(string extension)
        {
            return extension switch
            {
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                "webp" => "image/webp",
                "svg" => "image/svg+xml",
                "ico" => "image/x-icon",
                _ => null
            };
        }

        public async Task<List<string>> GetAccidentPhotoDataUrlsAsync(long sinisterId)
        {
            var documents = await GetDocumentsBySinisterIdAsync(sinisterId);
            var photos = documents.Where(d => string.Equals(d.TypeDocument, "Photo", StringComparison.OrdinalIgnoreCase)).ToList();
            if (!photos.Any())
            {
                return new List<string>();
            }

            var results = new List<string>();
            foreach (var photo in photos)
            {
                var extension = (photo.FileType ?? Path.GetExtension(photo.Name))?.TrimStart('.').ToLowerInvariant() ?? string.Empty;
                var mimeType = GetImageMimeType(extension);
                if (mimeType == null)
                {
                    continue;
                }

                var content = await GetFileContentAsync(photo.StreamId);
                if (content == null || content.Length == 0)
                {
                    continue;
                }

                var base64 = Convert.ToBase64String(content);
                results.Add($"data:{mimeType};base64,{base64}");
            }

            return results;
        }
    }
}

