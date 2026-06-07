using EmployeeDocuments.Module.Business;
using Project.Module.Business;

namespace Employee.Module.Business.Models
{
    /// <summary>
    /// Composite business model containing employee with related documents and project assignment
    /// </summary>
    public class EmployeeDetailBusinessModel
    {
        public List<EmployeeDocumentBusinessModel> Documents { get; set; } = new();
        public EmployeeBusinessModel Employee { get; set; } = new();
        public ProjectBusinessModel? Project { get; set; }
    }
}
