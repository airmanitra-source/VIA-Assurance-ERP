namespace Project.Module.Data.Models
{
    public class ProjectDataModel
    {
        public int ProjectID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Description { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}
