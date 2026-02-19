using Microsoft.AspNetCore.Mvc;
using EmployeeDocuments.Module;

namespace ClientApp.Controllers;

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

    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocument(IFormFile file, [FromForm] long employeeId, [FromForm] string? typeDocument)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        using var ms = new System.IO.MemoryStream();
        await file.CopyToAsync(ms);
        var fileContent = ms.ToArray();

        var streamId = await _documentModule.UploadAndLinkDocumentAsync(employeeId, file.FileName, fileContent, typeDocument);
        return Ok(new { StreamId = streamId, FileName = file.FileName });
    }

    [HttpDelete("{employeeId}/{streamId}")]
    public async Task<IActionResult> DeleteDocument(long employeeId, Guid streamId)
    {
        await _documentModule.RemoveDocumentAsync(employeeId, streamId);
        return NoContent();
    }

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
        // Normalize: remove dot, lower case
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
}
