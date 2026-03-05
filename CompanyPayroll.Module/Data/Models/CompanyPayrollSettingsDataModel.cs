namespace CompanyPayroll.Module.Data.Models
{
    public class CompanyPayrollSettingsDataModel
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
    }
}
