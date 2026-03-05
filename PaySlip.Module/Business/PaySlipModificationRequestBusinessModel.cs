using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Business
{
    public class PaySlipModificationRequestBusinessModel
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

        internal static PaySlipModificationRequestBusinessModel FromDataModel(PaySlipModificationRequestDataModel d)
        {
            return new PaySlipModificationRequestBusinessModel
            {
                Bonus = d.Bonus,
                Comments = d.Comments,
                CreatedDate = d.CreatedDate,
                EmployeeID = d.EmployeeID,
                IndemniteLogement = d.IndemniteLogement,
                IndemniteTransport = d.IndemniteTransport,
                OvertimeHours = d.OvertimeHours,
                PeriodID = d.PeriodID,
                PrimeScolarite = d.PrimeScolarite,
                RequestID = d.RequestID,
                ReviewedDate = d.ReviewedDate,
                Status = d.Status,
                TreiziemeMois = d.TreiziemeMois
            };
        }
    }
}
