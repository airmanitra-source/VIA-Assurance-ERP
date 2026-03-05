namespace EmployeePayroll.Module.Data.Models
{
    public class PayrollPeriodDataModel
    {
        public int PeriodID { get; set; }
        public long EntrepriseID { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime PeriodStart { get; set; }
        public string Status { get; set; } = "Draft";
    }
}
