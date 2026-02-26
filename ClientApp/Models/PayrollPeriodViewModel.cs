namespace ClientApp.Models
{
    public class PayrollPeriodViewModel
    {
        public int PeriodID { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string PeriodLabel => $"Du {PeriodStart:dd/MM/yyyy} au {PeriodEnd:dd/MM/yyyy}";
        public DateTime PeriodStart { get; set; }
        public string Status { get; set; } = "Draft";
    }
}
