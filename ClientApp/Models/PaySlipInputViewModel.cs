namespace ClientApp.Models
{
    public class PaySlipInputViewModel
    {
        public decimal? Bonus { get; set; }
        public long EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal? IndemniteLogement { get; set; }
        public decimal? IndemniteTransport { get; set; }
        public decimal? OvertimeHours { get; set; }
        public decimal? PrimeScolarite { get; set; }
    }
}
