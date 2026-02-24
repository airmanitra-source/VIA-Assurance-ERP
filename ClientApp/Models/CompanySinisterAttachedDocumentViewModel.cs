using Microsoft.AspNetCore.Components.Forms;

namespace ClientApp.Models
{
    public class CompanySinisterAttachedDocumentViewModel
    {
        public IBrowserFile? File { get; set; }

        public string Name => File?.Name ?? "Unknown";

        public string TypeDocument { get; set; } = "Photo";
    }
}
