using Employee.Module.Data.Models;

namespace Employee.Module.Business
{
    public class PayrollPeriodBusinessModel
    {
        public int PeriodID { get; set; }
        public long EntrepriseID { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime PeriodStart { get; set; }
        public string Status { get; set; } = "Draft";

        internal static PayrollPeriodBusinessModel FromDataModel(PayrollPeriodDataModel d)
        {
            return new PayrollPeriodBusinessModel
            {
                EntrepriseID = d.EntrepriseID,
                PaymentDate = d.PaymentDate,
                PeriodEnd = d.PeriodEnd,
                PeriodID = d.PeriodID,
                PeriodStart = d.PeriodStart,
                Status = d.Status
            };
        }
    }
}
