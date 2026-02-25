namespace Employee.Module.Data.Models
{
    public class EmployeeTimesheetDataModel
    {
        public long TimesheetID { get; set; }
        public string? Comments { get; set; }
        public long EmployeeID { get; set; }
        public decimal HoursWorked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal OvertimeHours { get; set; }
        public long? ProjectID { get; set; }
        public string Status { get; set; } = "Draft";
        public string? TaskDescription { get; set; }
        public DateTime WorkDate { get; set; }
    }
}
