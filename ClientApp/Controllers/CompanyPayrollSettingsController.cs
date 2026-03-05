using ClientApp.Models;
using CompanyPayroll.Module;
using CompanyPayroll.Module.Business;
using Employee.Module;
using PaySlip.Module;

namespace ClientApp.Controllers
{
    public class CompanyPayrollSettingsController
    {
        private readonly ICompanyPayrollModule _companyPayrollModule;
        private readonly IEmployeeModule _employeeModule;
        private readonly IPaySlipModule _paySlipModule;

        public CompanyPayrollSettingsController(
            ICompanyPayrollModule companyPayrollModule,
            IEmployeeModule employeeModule,
            IPaySlipModule paySlipModule)
        {
            _companyPayrollModule = companyPayrollModule;
            _employeeModule = employeeModule;
            _paySlipModule = paySlipModule;
        }

        /// <summary>
        /// REST: Show - Get current payroll settings for an enterprise
        /// </summary>
        public async Task<CompanyPayrollSettingsViewModel> Show(long enterpriseId)
        {
            var settings = await _companyPayrollModule.GetSettingsAsync(enterpriseId);
            return MapToViewModel(settings);
        }

        /// <summary>
        /// REST: Store - Save payroll settings and recalculate all draft payslips
        /// </summary>
        public async Task Store(CompanyPayrollSettingsViewModel viewModel)
        {
            var business = MapToBusinessModel(viewModel);
            await _companyPayrollModule.SetSettingsAsync(business);

            var employees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(viewModel.EntrepriseID);
            foreach (var emp in employees.Where(e => e.IsActive))
            {
                await _paySlipModule.SetRecalculateDraftPaySlipsForEmployeeAsync(emp.EmployeeID, viewModel.EntrepriseID);
            }
        }

        private static CompanyPayrollSettingsViewModel MapToViewModel(CompanyPayrollSettingsBusinessModel b)
        {
            return new CompanyPayrollSettingsViewModel
            {
                CnapsComplementaryEmployerRate = b.CnapsComplementaryEmployerRate,
                CnapsComplementaryRate = b.CnapsComplementaryRate,
                CnapsEmployeeRate = b.CnapsEmployeeRate,
                CnapsEmployerRate = b.CnapsEmployerRate,
                EntrepriseID = b.EntrepriseID,
                IrsaDependentReduction = b.IrsaDependentReduction,
                IrsaMinimum = b.IrsaMinimum,
                MaxOvertimeHoursPerWeek = b.MaxOvertimeHoursPerWeek,
                OstieEmployeeRate = b.OstieEmployeeRate,
                OstieEmployerRate = b.OstieEmployerRate,
                OvertimeRateMultiplier = b.OvertimeRateMultiplier,
                RequireDoubleEntry = b.RequireDoubleEntry,
                SettingsID = b.SettingsID
            };
        }

        private static CompanyPayrollSettingsBusinessModel MapToBusinessModel(CompanyPayrollSettingsViewModel v)
        {
            return new CompanyPayrollSettingsBusinessModel
            {
                CnapsComplementaryEmployerRate = v.CnapsComplementaryEmployerRate,
                CnapsComplementaryRate = v.CnapsComplementaryRate,
                CnapsEmployeeRate = v.CnapsEmployeeRate,
                CnapsEmployerRate = v.CnapsEmployerRate,
                EntrepriseID = v.EntrepriseID,
                IrsaDependentReduction = v.IrsaDependentReduction,
                IrsaMinimum = v.IrsaMinimum,
                MaxOvertimeHoursPerWeek = v.MaxOvertimeHoursPerWeek,
                OstieEmployeeRate = v.OstieEmployeeRate,
                OstieEmployerRate = v.OstieEmployerRate,
                OvertimeRateMultiplier = v.OvertimeRateMultiplier,
                RequireDoubleEntry = v.RequireDoubleEntry,
                SettingsID = v.SettingsID
            };
        }
    }
}
