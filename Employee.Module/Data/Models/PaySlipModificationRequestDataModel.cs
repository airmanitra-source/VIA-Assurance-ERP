namespace Employee.Module.Data.Models
{
    public class PaySlipModificationRequestDataModel
    {
        public int RequestID { get; set; }
        public decimal? Bonus { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public long EmployeeID { get; set; }
        public decimal? IndemniteLogement { get; set; }
        public decimal? IndemniteTransport { get; set; }
        public decimal? OvertimeHours { get; set; }
        public int PeriodID { get; set; }
        public decimal? PrimeScolarite { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal? TreiziemeMois { get; set; }
    }
}
