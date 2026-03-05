using CompanyPayroll.Module.Data.Models;

namespace CompanyPayroll.Module.Business
{
    public class CompanyPayrollSettingsBusinessModel
    {
        public int SettingsID { get; set; }
        public decimal CnapsComplementaryEmployerRate { get; set; }
        public decimal CnapsComplementaryRate { get; set; }
        public decimal CnapsEmployeeRate { get; set; }
        public decimal CnapsEmployerRate { get; set; }
        public long EntrepriseID { get; set; }
        public decimal IrsaDependentReduction { get; set; }
        public decimal IrsaMinimum { get; set; }
        public int MaxOvertimeHoursPerWeek { get; set; }
        public decimal OstieEmployeeRate { get; set; }
        public decimal OstieEmployerRate { get; set; }
        public decimal OvertimeRateMultiplier { get; set; }
        public bool RequireDoubleEntry { get; set; }

        internal static CompanyPayrollSettingsBusinessModel FromDataModel(CompanyPayrollSettingsDataModel d)
        {
            return new CompanyPayrollSettingsBusinessModel
            {
                CnapsComplementaryEmployerRate = d.CnapsComplementaryEmployerRate,
                CnapsComplementaryRate = d.CnapsComplementaryRate,
                CnapsEmployeeRate = d.CnapsEmployeeRate,
                CnapsEmployerRate = d.CnapsEmployerRate,
                EntrepriseID = d.EntrepriseID,
                IrsaDependentReduction = d.IrsaDependentReduction,
                IrsaMinimum = d.IrsaMinimum,
                MaxOvertimeHoursPerWeek = d.MaxOvertimeHoursPerWeek,
                OstieEmployeeRate = d.OstieEmployeeRate,
                OstieEmployerRate = d.OstieEmployerRate,
                OvertimeRateMultiplier = d.OvertimeRateMultiplier,
                RequireDoubleEntry = d.RequireDoubleEntry,
                SettingsID = d.SettingsID
            };
        }
    }
}
