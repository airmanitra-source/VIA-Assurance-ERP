using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Business.Models
{
    public class PaySlipLineBusinessModel
    {
        public long LineID { get; set; }
        public decimal? Base { get; set; }
        public long EmployeeID { get; set; }
        public decimal EmployeeDeduction { get; set; }
        public decimal EmployerContribution { get; set; }
        public decimal GainAmount { get; set; }
        public string Libelle { get; set; } = string.Empty;
        public string LineType { get; set; } = string.Empty;
        public decimal? Nombre { get; set; }
        public int PayrollID { get; set; }
        public int PeriodID { get; set; }
        public string Rubrique { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public decimal? Taux { get; set; }

        internal static PaySlipLineBusinessModel FromDataModel(PaySlipLineDataModel d)
        {
            return new PaySlipLineBusinessModel
            {
                Base = d.Base,
                EmployeeDeduction = d.EmployeeDeduction,
                EmployeeID = d.EmployeeID,
                EmployerContribution = d.EmployerContribution,
                GainAmount = d.GainAmount,
                Libelle = d.Libelle,
                LineID = d.LineID,
                LineType = d.LineType,
                Nombre = d.Nombre,
                PayrollID = d.PayrollID,
                PeriodID = d.PeriodID,
                Rubrique = d.Rubrique,
                SortOrder = d.SortOrder,
                Taux = d.Taux
            };
        }
    }
}
