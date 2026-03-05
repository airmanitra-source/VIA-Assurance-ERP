using CompanyPayroll.Module.Business;
using CompanyPayroll.Module.Data.Models;
using CompanyPayroll.Module.Data.Providers;

namespace CompanyPayroll.Module
{
    public class CompanyPayrollModule : ICompanyPayrollModule
    {
        private readonly ICompanyPayrollSettingsReadOnly _settingsReadOnly;
        private readonly ICompanyPayrollSettingsReadWrite _settingsReadWrite;

        public CompanyPayrollModule(
            ICompanyPayrollSettingsReadOnly settingsReadOnly,
            ICompanyPayrollSettingsReadWrite settingsReadWrite)
        {
            _settingsReadOnly = settingsReadOnly;
            _settingsReadWrite = settingsReadWrite;
        }

        public async Task<CompanyPayrollSettingsBusinessModel> GetSettingsAsync(long enterpriseId)
        {
            var data = await _settingsReadOnly.ReadByEnterpriseIdAsync(enterpriseId);
            if (data != null)
                return CompanyPayrollSettingsBusinessModel.FromDataModel(data);

            return new CompanyPayrollSettingsBusinessModel
            {
                CnapsComplementaryEmployerRate = 0.035m,
                CnapsComplementaryRate = 0.01m,
                CnapsEmployeeRate = 0.01m,
                CnapsEmployerRate = 0.13m,
                EntrepriseID = enterpriseId,
                IrsaDependentReduction = 2000m,
                IrsaMinimum = 3000m,
                MaxOvertimeHoursPerWeek = 20,
                OstieEmployeeRate = 0.01m,
                OstieEmployerRate = 0.05m,
                OvertimeRateMultiplier = 1.5m,
                RequireDoubleEntry = false
            };
        }

        public async Task SetSettingsAsync(CompanyPayrollSettingsBusinessModel settings)
        {
            var dataModel = new CompanyPayrollSettingsDataModel
            {
                CnapsComplementaryEmployerRate = settings.CnapsComplementaryEmployerRate,
                CnapsComplementaryRate = settings.CnapsComplementaryRate,
                CnapsEmployeeRate = settings.CnapsEmployeeRate,
                CnapsEmployerRate = settings.CnapsEmployerRate,
                EntrepriseID = settings.EntrepriseID,
                IrsaDependentReduction = settings.IrsaDependentReduction,
                IrsaMinimum = settings.IrsaMinimum,
                MaxOvertimeHoursPerWeek = settings.MaxOvertimeHoursPerWeek,
                OstieEmployeeRate = settings.OstieEmployeeRate,
                OstieEmployerRate = settings.OstieEmployerRate,
                OvertimeRateMultiplier = settings.OvertimeRateMultiplier,
                RequireDoubleEntry = settings.RequireDoubleEntry,
                SettingsID = settings.SettingsID
            };

            if (settings.SettingsID > 0)
            {
                await _settingsReadWrite.UpdateSettingsAsync(dataModel);
            }
            else
            {
                settings.SettingsID = await _settingsReadWrite.CreateSettingsAsync(dataModel);
            }
        }
    }
}
