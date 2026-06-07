using EmployeeDocuments.Module.Business;
using EmployeeDocuments.Module.Data.Models;
using EmployeeDocuments.Module.Data.Providers;

namespace EmployeeDocuments.Module
{
    public class EmployeeDocumentModule: IEmployeeDocumentModule
    {
        private readonly IEmployeeDocumentReadOnlyDataProvider _employeeDocumentReadOnly;
        private readonly IEmployeeDocumentReadWriteDataProvider _employeeDocumentReadWrite;

        public EmployeeDocumentModule(IEmployeeDocumentReadOnlyDataProvider employeeDocumentReadOnly, IEmployeeDocumentReadWriteDataProvider employeeDocumentReadWrite)
        {
            _employeeDocumentReadOnly = employeeDocumentReadOnly;
            _employeeDocumentReadWrite = employeeDocumentReadWrite;
        }

        public async Task<List<EmployeeDocumentBusinessModel>> GetAllDocumentsAsync()
        {
            var dataModels = await _employeeDocumentReadOnly.ReadAllDocumentsAsync();
            return dataModels.Select(ConvertToBusinessModel).ToList();
        }

        public async Task<EmployeeDocumentBusinessModel?> GetDocumentByIdAsync(Guid streamId)
        {
            var dataModel = await _employeeDocumentReadOnly.ReadDocumentByIdAsync(streamId);
            return dataModel != null ? ConvertToBusinessModel(dataModel) : null;
        }

        public async Task<byte[]?> GetFileContentAsync(Guid streamId)
        {
            return await _employeeDocumentReadOnly.ReadFileContentAsync(streamId);
        }

        public async Task<List<EmployeeDocumentBusinessModel>> GetDocumentsByEmployeeIdAsync(long employeeId)
        {
            var dataModels = await _employeeDocumentReadOnly.ReadDocumentsByEmployeeIdAsync(employeeId);
            return dataModels.Select(ConvertToBusinessModel).ToList();
        }

        public async Task RemoveDocumentAsync(long employeeId, Guid streamId)
        {
            await _employeeDocumentReadWrite.DeleteEmployeeDocumentAsync(employeeId, streamId);
        }

        public async Task<Guid> UploadAndLinkDocumentAsync(long employeeId, string fileName, byte[] fileContent, string? typeDocument)
        {
            var streamId = await _employeeDocumentReadWrite.AddEmployeeFileIntoDocumentsAsync(fileName, fileContent);
            await _employeeDocumentReadWrite.AddEmployeeDocumentAsync(employeeId, streamId, typeDocument);
            return streamId;
        }

        private static EmployeeDocumentBusinessModel ConvertToBusinessModel(EmployeeDocumentDataModel dataModel)
        {
            return new EmployeeDocumentBusinessModel
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
                IsTemporary = dataModel.IsTemporary,
                TypeDocument = dataModel.TypeDocument
            };
        }
    }
}

