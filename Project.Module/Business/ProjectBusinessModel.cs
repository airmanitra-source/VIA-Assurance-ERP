namespace Project.Module.Business
{
    public class ProjectBusinessModel
    {
        public int ProjectID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Description { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = "Active";

        internal static ProjectBusinessModel FromDataModel(Data.Models.ProjectDataModel d)
        {
            return new ProjectBusinessModel
            {
                CreatedDate = d.CreatedDate,
                Description = d.Description,
                EndDate = d.EndDate,
                ProjectID = d.ProjectID,
                ProjectName = d.ProjectName,
                StartDate = d.StartDate,
                Status = d.Status
            };
        }
    }
}
