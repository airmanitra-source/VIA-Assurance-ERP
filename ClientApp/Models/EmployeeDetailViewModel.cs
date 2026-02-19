using System.Collections.Generic;

namespace ClientApp.Models
{
    public class EmployeeDetailViewModel
    {
        public List<AttachedDocumentViewModel> Documents { get; set; } = new();

        public EmployeeViewModel Employee { get; set; } = new();
    }
}
