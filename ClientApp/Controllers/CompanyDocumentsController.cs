using ClientApp.Models;
using CompanyDocuments.Module;
using CompanyDocuments.Module.Business;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "directeur,RH,auditeur,admin,developer")]
    public class CompanyDocumentsController : ControllerBase
    {
        private readonly ICompanyDocumentModule _documentModule;

        public CompanyDocumentsController(ICompanyDocumentModule documentModule)
        {
            _documentModule = documentModule;
        }

        // --- Service methods (called directly by Blazor components) ---

        public async Task<List<CompanyDocumentViewModel>> Index(long enterpriseId)
        {
            var items = await _documentModule.GetDocumentsByEntrepriseIdAsync(enterpriseId);
            return items.Select(MapToViewModel).ToList();
        }

        public async Task<CompanyDocumentViewModel?> Show(Guid streamId)
        {
            var doc = await _documentModule.GetDocumentByIdAsync(streamId);
            return doc != null ? MapToViewModel(doc) : null;
        }

        public async Task<Guid> Store(long enterpriseId, string fileName, byte[] content, string? typeDocument)
        {
            return await _documentModule.UploadAndLinkDocumentAsync(enterpriseId, fileName, content, typeDocument);
        }

        public async Task Destroy(long enterpriseId, Guid streamId)
        {
            await _documentModule.RemoveDocumentAsync(enterpriseId, streamId);
        }

        public async Task Sign(long enterpriseId, Guid streamId, string signerName, string signatureDataUrl)
        {
            await _documentModule.SignDocumentAsync(enterpriseId, streamId, signerName, signatureDataUrl);
        }

        // --- REST API Endpoints ---

        [HttpDelete("{entrepriseId}/{streamId}")]
        public async Task<IActionResult> DeleteDocument(long entrepriseId, Guid streamId)
        {
            await _documentModule.RemoveDocumentAsync(entrepriseId, streamId);
            return NoContent();
        }

        [HttpGet("{streamId}/content")]
        public async Task<IActionResult> GetDocumentContent([FromRoute] Guid streamId, [FromQuery] bool download = false)
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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file, [FromForm] long entrepriseId, [FromForm] string? typeDocument)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            using var ms = new System.IO.MemoryStream();
            await file.CopyToAsync(ms);
            var fileContent = ms.ToArray();

            var streamId = await _documentModule.UploadAndLinkDocumentAsync(entrepriseId, file.FileName, fileContent, typeDocument);
            return Ok(new { StreamId = streamId, FileName = file.FileName });
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
        private CompanyDocumentViewModel MapToViewModel(CompanyDocumentBusinessModel m)
        {
            return new CompanyDocumentViewModel
            {
                StreamId = m.StreamId,
                CachedFileSize = m.CachedFileSize,
                Children = m.Children?.Select(MapToViewModel).ToList() ?? new List<CompanyDocumentViewModel>(),
                CreationTime = m.CreationTime,
                FileType = m.FileType,
                IsArchive = m.IsArchive,
                IsDirectory = m.IsDirectory,
                IsHidden = m.IsHidden,
                IsOffline = m.IsOffline,
                IsReadonly = m.IsReadonly,
                IsSigned = m.IsSigned,
                IsSystem = m.IsSystem,
                IsTemporary = m.IsTemporary,
                LastAccessTime = m.LastAccessTime,
                LastWriteTime = m.LastWriteTime,
                Name = m.Name,
                SignedDate = m.SignedDate,
                TypeDocument = m.TypeDocument
            };
        }
        #endregion
    }
}
