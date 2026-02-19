using EmployeeDocuments.Module.Business;
using Microsoft.AspNetCore.Components.Forms;

namespace ClientApp.Models
{
    public class AttachedDocumentViewModel
    {
        public IBrowserFile? File { get; set; }

        public EmployeeDocumentBusinessModel? ExistingDocument { get; set; }

        public string TypeDocument { get; set; } = string.Empty;

        public string Name => ExistingDocument?.Name ?? File?.Name ?? "Unknown";
    }

}