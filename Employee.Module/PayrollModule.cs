using Employee.Module.Business;
using Employee.Module.Data.Models;
using Employee.Module.Data.Providers;

namespace Employee.Module
{
    public class PayrollModule : IPayrollModule
    {
        private readonly ICompanyPayrollSettingsReadOnly _settingsReadOnly;
        private readonly IEmployeeReadWrite _employeeReadWrite;
        private readonly IPayrollPeriodReadOnly _periodReadOnly;
        private readonly IPayrollPeriodReadWrite _periodReadWrite;
        private readonly IPaySlipLineReadOnly _lineReadOnly;
        private readonly IPaySlipLineReadWrite _lineReadWrite;
        private readonly IEmployeePayrollReadWrite _payrollReadWrite;

        public PayrollModule(
            ICompanyPayrollSettingsReadOnly settingsReadOnly,
            IEmployeeReadWrite employeeReadWrite,
            IPayrollPeriodReadOnly periodReadOnly,
            IPayrollPeriodReadWrite periodReadWrite,
            IPaySlipLineReadOnly lineReadOnly,
            IPaySlipLineReadWrite lineReadWrite,
            IEmployeePayrollReadWrite payrollReadWrite)
        {
            _settingsReadOnly = settingsReadOnly;
            _employeeReadWrite = employeeReadWrite;
            _periodReadOnly = periodReadOnly;
            _periodReadWrite = periodReadWrite;
            _lineReadOnly = lineReadOnly;
            _lineReadWrite = lineReadWrite;
            _payrollReadWrite = payrollReadWrite;
        }

        public async Task<int> AddPeriodAsync(long enterpriseId, DateTime periodStart, DateTime periodEnd)
        {
            var dataModel = new PayrollPeriodDataModel
            {
                EntrepriseID = enterpriseId,
                PeriodEnd = periodEnd,
                PeriodStart = periodStart,
                Status = "Draft"
            };
            return await _periodReadWrite.CreatePeriodAsync(dataModel);
        }

        public async Task<int> AddPaySlipAsync(PaySlipBusinessModel paySlip, DateTime paymentDate, int periodMonth, int periodYear)
        {
            // Create payroll record
            var payrollData = new EmployeePayrollDataModel
            {
                BaseSalary = paySlip.TotalGains,
                Bonus = 0,
                Deductions = paySlip.TotalCotisationsEmployee + paySlip.Irsa,
                EmployeeID = paySlip.EmployeeID,
                NetSalary = paySlip.NetAPayer,
                PaymentDate = paymentDate,
                PaymentMethod = "BankTransfer",
                PayPeriodMonth = periodMonth,
                PayPeriodYear = periodYear
            };
            int payrollId = await _payrollReadWrite.CreatePayrollAsync(payrollData);

            // Update payrollId on all lines
            var lineDataModels = paySlip.Lines.Select(l => new PaySlipLineDataModel
            {
                Base = l.Base,
                EmployeeDeduction = l.EmployeeDeduction,
                EmployeeID = l.EmployeeID,
                EmployerContribution = l.EmployerContribution,
                GainAmount = l.GainAmount,
                Libelle = l.Libelle,
                LineType = l.LineType,
                Nombre = l.Nombre,
                PayrollID = payrollId,
                PeriodID = l.PeriodID,
                Rubrique = l.Rubrique,
                SortOrder = l.SortOrder,
                Taux = l.Taux
            }).ToList();

            await _lineReadWrite.CreateLinesAsync(lineDataModels);

            return payrollId;
        }

        public async Task<List<PayrollPeriodBusinessModel>> GetPeriodsByEnterpriseAsync(long enterpriseId)
        {
            var items = await _periodReadOnly.ReadByEnterpriseIdAsync(enterpriseId);
            return items.Select(PayrollPeriodBusinessModel.FromDataModel).ToList();
        }

        public async Task<CompanyPayrollSettingsBusinessModel> GetSettingsAsync(long enterpriseId)
        {
            var data = await _settingsReadOnly.ReadByEnterpriseIdAsync(enterpriseId);
            if (data != null)
                return CompanyPayrollSettingsBusinessModel.FromDataModel(data);

            // Return defaults if no settings configured
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
                OvertimeRateMultiplier = 1.5m
            };
        }

        public async Task<PaySlipBusinessModel> GetPaySlipAsync(long employeeId, int periodId, long enterpriseId)
        {
            var settings = await GetSettingsAsync(enterpriseId);

            // Check existing lines
            var existingLines = await _lineReadOnly.ReadByPeriodAndEmployeeAsync(periodId, employeeId);
            if (existingLines.Any())
            {
                var employee = await _employeeReadWrite.ReadEmployeeByIdAsync((int)employeeId);
                var slip = new PaySlipBusinessModel
                {
                    BankAccountNumber = employee?.BankAccountNumber,
                    Classification = employee?.Classification,
                    Dependents = employee?.Dependents ?? 0,
                    EmployeeID = employeeId,
                    EmployeeName = employee != null ? $"{employee.Prenom} {employee.Nom}" : string.Empty,
                    Lines = existingLines.Select(PaySlipLineBusinessModel.FromDataModel).ToList(),
                    NumeroCnaps = employee?.NumeroCnaps,
                    PeriodID = periodId,
                    Poste = employee?.NomPoste
                };
                return slip;
            }

            // Generate new payslip from employee base salary
            var emp = await _employeeReadWrite.ReadEmployeeByIdAsync((int)employeeId);
            if (emp == null)
                throw new InvalidOperationException($"Employee {employeeId} not found");

            var businessEmp = new EmployeeBusinessModel
            {
                BankAccountNumber = emp.BankAccountNumber,
                Classification = emp.Classification,
                Dependents = emp.Dependents,
                EmployeeID = emp.EmployeeID,
                Nom = emp.Nom,
                NomPoste = emp.NomPoste,
                NumeroCnaps = emp.NumeroCnaps,
                Prenom = emp.Prenom,
                Salaire = emp.Salaire
            };

            return PaySlipBusinessModel.Generate(businessEmp, settings, periodId, 0);
        }

        public async Task SetPeriodStatusAsync(int periodId, string status)
        {
            await _periodReadWrite.UpdatePeriodStatusAsync(periodId, status);
        }
    }
}
