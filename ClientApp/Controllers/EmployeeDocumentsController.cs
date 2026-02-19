using ClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using EmployeeDocuments.Module;
using EmployeeDocuments.Module.Business;

namespace ClientApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "directeur,RH,auditeur,admin,developer")]
    public class EmployeeDocumentsController : ControllerBase
    {
        private readonly IEmployeeDocumentModule _documentModule;

        public EmployeeDocumentsController(IEmployeeDocumentModule documentModule)
        {
            _documentModule = documentModule;
        }

        // --- Service methods (called directly by Blazor components) ---

        public async Task<List<EmployeeDocumentViewModel>> Index(long? employeeId = null)
        {
            List<EmployeeDocumentBusinessModel> businessModels;
            if (employeeId.HasValue)
            {
                businessModels = await _documentModule.GetDocumentsByEmployeeIdAsync(employeeId.Value);
            }
            else
            {
                businessModels = await _documentModule.GetAllDocumentsAsync();
            }
            return businessModels.Select(MapToViewModel).ToList();
        }

        public async Task<EmployeeDocumentViewModel?> Show(Guid streamId)
        {
            var doc = await _documentModule.GetDocumentByIdAsync(streamId);
            return doc != null ? MapToViewModel(doc) : null;
        }

        public async Task<Guid> Store(long employeeId, string fileName, byte[] content, string? typeDocument)
        {
            return await _documentModule.UploadAndLinkDocumentAsync(employeeId, fileName, content, typeDocument);
        }

        public async Task Destroy(long employeeId, Guid streamId)
        {
            await _documentModule.RemoveDocumentAsync(employeeId, streamId);
        }

        public async Task<byte[]?> GetContentBytes(Guid streamId)
        {
            return await _documentModule.GetFileContentAsync(streamId);
        }

        // --- REST API Endpoints (used for Serving Content via URL for Preview) ---

        [HttpGet("{streamId}/content")]
        public async Task<IActionResult> GetDocumentContent(Guid streamId, [FromQuery] bool download = false)
        {
            var document = await _documentModule.GetDocumentByIdAsync(streamId);
            if (document == null || document.IsDirectory)
            {
                return NotFound();
            }

            var content = await _documentModule.GetFileContentAsync(streamId);
            if (content == null)
            {
                return NotFound();
            }

            var mimeType = GetMimeType(document.FileType);
            
            if (download)
            {
                return File(content, mimeType, document.Name);
            }
            
            return File(content, mimeType);
        }

        private string GetMimeType(string? fileType)
        {
            var ext = fileType?.TrimStart('.').ToLowerInvariant();
            return ext switch
            {
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                "webp" => "image/webp",
                "svg" => "image/svg+xml",
                "ico" => "image/x-icon",
                "pdf" => "application/pdf",
                "txt" or "md" or "cs" or "sql" or "json" or "xml" or "html" or "css" or "js" or "config" or "ini" or "yaml" or "yml" or "log" or "csv" => "text/plain",
                "mp4" => "video/mp4",
                "webm" => "video/webm",
                "ogg" => "video/ogg",
                "mov" => "video/quicktime",
                "mp3" => "audio/mpeg",
                "wav" => "audio/wav",
                "m4a" => "audio/mp4",
                _ => "application/octet-stream"
            };
        }

        #region Mapping
        private EmployeeDocumentViewModel MapToViewModel(EmployeeDocumentBusinessModel businessModel)
        {
            return new EmployeeDocumentViewModel
            {
                StreamId = businessModel.StreamId,
                CachedFileSize = businessModel.CachedFileSize,
                CreationTime = businessModel.CreationTime,
                FileType = businessModel.FileType,
                IsArchive = businessModel.IsArchive,
                IsDirectory = businessModel.IsDirectory,
                IsHidden = businessModel.IsHidden,
                IsOffline = businessModel.IsOffline,
                IsReadonly = businessModel.IsReadonly,
                IsSystem = businessModel.IsSystem,
                IsTemporary = businessModel.IsTemporary,
                LastAccessTime = businessModel.LastAccessTime,
                LastWriteTime = businessModel.LastWriteTime,
                Name = businessModel.Name,
                ParentPathLocator = businessModel.ParentPathLocator,
                PathLocator = businessModel.PathLocator,
                Children = businessModel.Children?.Select(MapToViewModel).ToList() ?? new List<EmployeeDocumentViewModel>()
            };
        }
        #endregion
    }
}
