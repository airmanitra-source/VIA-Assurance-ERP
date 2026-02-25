namespace Employee.Module.Business
{
    public class EmployeeTimesheetBusinessModel
    {
        public long TimesheetID { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public long EmployeeID { get; set; }
        public decimal HoursWorked { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal OvertimeHours { get; set; }
        public long? ProjectID { get; set; }
        public string Status { get; set; } = "Draft";
        public string? TaskDescription { get; set; }
        public DateTime WorkDate { get; set; }

        internal static EmployeeTimesheetBusinessModel FromDataModel(Data.Models.EmployeeTimesheetDataModel d)
        {
            return new EmployeeTimesheetBusinessModel
            {
                Comments = d.Comments,
                CreatedDate = d.CreatedDate,
                EmployeeID = d.EmployeeID,
                HoursWorked = d.HoursWorked,
                ModifiedDate = d.ModifiedDate,
                OvertimeHours = d.OvertimeHours,
                ProjectID = d.ProjectID,
                Status = d.Status,
                TaskDescription = d.TaskDescription,
                TimesheetID = d.TimesheetID,
                WorkDate = d.WorkDate
            };
        }
    }
}
