using ClientApp.Models;
using CompanyDocuments.Module;
using CompanyDocuments.Module.Business;
using Microsoft.AspNetCore.Mvc;
using Company.Fleet.Module;
using Company.Warehouse.Module;
using Company.Transportation.Module;

namespace ClientApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "directeur,RH,auditeur,admin,developer")]
    public class CompanyDocumentsController : ControllerBase
    {
        private readonly ICompanyDocumentModule _documentModule;
        private readonly ICompanyFleetModule _fleetModule;
        private readonly ICompanyWarehouseModule _warehouseModule;
        private readonly ICompanyTransportationModule _transportationModule;

        public CompanyDocumentsController(
            ICompanyDocumentModule documentModule,
            ICompanyFleetModule fleetModule,
            ICompanyWarehouseModule warehouseModule,
            ICompanyTransportationModule transportationModule)
        {
            _documentModule = documentModule;
            _fleetModule = fleetModule;
            _warehouseModule = warehouseModule;
            _transportationModule = transportationModule;
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
            // 1. Signer le document
            await _documentModule.SignDocumentAsync(enterpriseId, streamId, signerName, signatureDataUrl);
            
            // 2. Récupérer le document pour vérifier s'il est lié à un actif
            var document = await _documentModule.GetDocumentByIdAsync(streamId);
            
            if (document == null)
            {
                throw new InvalidOperationException("Document introuvable après signature.");
            }

            // 3. Marquer l'actif comme assuré selon le type
            try
            {
                if (document.EntrepriseFleetID.HasValue)
                {
                    // Véhicule de la flotte
                    await _fleetModule.MarkAsInsuredAsync(document.EntrepriseFleetID.Value);
                }
                else if (document.EntrepriseWarehouseID.HasValue)
                {
                    // Entrepôt
                    await _warehouseModule.MarkAsInsuredAsync(document.EntrepriseWarehouseID.Value);
                }
                else if (document.EntrepriseMerchandiseTransportationID.HasValue)
                {
                    // Transport de marchandises
                    await _transportationModule.MarkAsInsuredAsync(document.EntrepriseMerchandiseTransportationID.Value);
                }
                // Si aucun ID d'actif, c'est un document générique (pas de marquage nécessaire)
            }
            catch (InvalidOperationException ex)
            {
                // L'actif n'existe pas ou autre erreur - on log mais on ne bloque pas la signature
                Console.WriteLine($"Erreur lors du marquage de l'actif comme assuré: {ex.Message}");
                // On pourrait aussi laisser l'exception remonter si on veut être strict
                throw new InvalidOperationException(
                    $"Document signé avec succès, mais erreur lors du marquage de l'actif: {ex.Message}", ex);
            }
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
