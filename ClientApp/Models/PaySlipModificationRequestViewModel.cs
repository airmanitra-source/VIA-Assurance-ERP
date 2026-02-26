namespace ClientApp.Models
{
    public class PaySlipModificationRequestViewModel
    {
        public int RequestID { get; set; }
        public decimal? Bonus { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public long EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal? IndemniteLogement { get; set; }
        public decimal? IndemniteTransport { get; set; }
        public decimal? OvertimeHours { get; set; }
        public int PeriodID { get; set; }
        public decimal? PrimeScolarite { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal? TreiziemeMois { get; set; }
    }
}
