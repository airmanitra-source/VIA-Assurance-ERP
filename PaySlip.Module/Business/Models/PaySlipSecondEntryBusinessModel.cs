using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Business.Models
{
    public class PaySlipSecondEntryBusinessModel
    {
        public int SecondEntryID { get; set; }
        public decimal? Bonus { get; set; }
        public DateTime CreatedDate { get; set; }
        public long EmployeeID { get; set; }
        public decimal? IndemniteLogement { get; set; }
        public decimal? IndemniteTransport { get; set; }
        public decimal? OvertimeHours { get; set; }
        public int PeriodID { get; set; }
        public decimal? PrimeScolarite { get; set; }
        public decimal? TreiziemeMois { get; set; }

        internal static PaySlipSecondEntryBusinessModel FromDataModel(PaySlipSecondEntryDataModel d)
        {
            return new PaySlipSecondEntryBusinessModel
            {
                Bonus = d.Bonus,
                CreatedDate = d.CreatedDate,
                EmployeeID = d.EmployeeID,
                IndemniteLogement = d.IndemniteLogement,
                IndemniteTransport = d.IndemniteTransport,
                OvertimeHours = d.OvertimeHours,
                PeriodID = d.PeriodID,
                PrimeScolarite = d.PrimeScolarite,
                SecondEntryID = d.SecondEntryID,
                TreiziemeMois = d.TreiziemeMois
            };
        }
    }
}
