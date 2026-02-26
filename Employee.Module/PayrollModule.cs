using Employee.Module.Business;
using Employee.Module.Data.Models;
using Employee.Module.Data.Providers;

namespace Employee.Module
{
    public class PayrollModule : IPayrollModule
    {
        private readonly ICompanyPayrollSettingsReadOnly _settingsReadOnly;
        private readonly IEmployeeReadWrite _employeeReadWrite;
        private readonly IPaySlipModificationRequestReadWrite _modificationRequestWrite;
        private readonly IPaySlipModificationRequestReadOnly _modificationRequestRead;
        private readonly IEmployeePayrollReadWrite _payrollReadWrite;
        private readonly IPaySlipReadWrite _paySlipUpdate;
        private readonly IPayrollPeriodReadOnly _periodReadOnly;
        private readonly IPayrollPeriodReadWrite _periodReadWrite;
        private readonly IPaySlipLineReadOnly _lineReadOnly;
        private readonly IPaySlipLineReadWrite _lineReadWrite;

        public PayrollModule(
            ICompanyPayrollSettingsReadOnly settingsReadOnly,
            IEmployeeReadWrite employeeReadWrite,
            IPayrollPeriodReadOnly periodReadOnly,
            IPayrollPeriodReadWrite periodReadWrite,
            IPaySlipLineReadOnly lineReadOnly,
            IPaySlipLineReadWrite lineReadWrite,
            IEmployeePayrollReadWrite payrollReadWrite,
            IPaySlipReadWrite paySlipUpdate,
            IPaySlipModificationRequestReadOnly modificationRequestRead,
            IPaySlipModificationRequestReadWrite modificationRequestWrite)
        {
            _settingsReadOnly = settingsReadOnly;
            _employeeReadWrite = employeeReadWrite;
            _periodReadOnly = periodReadOnly;
            _periodReadWrite = periodReadWrite;
            _lineReadOnly = lineReadOnly;
            _lineReadWrite = lineReadWrite;
            _payrollReadWrite = payrollReadWrite;
            _paySlipUpdate = paySlipUpdate;
            _modificationRequestRead = modificationRequestRead;
            _modificationRequestWrite = modificationRequestWrite;
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

        public async Task<PaySlipBusinessModel?> GetSavedPaySlipAsync(long employeeId, int periodId)
        {
            var existingLines = await _lineReadOnly.ReadByPeriodAndEmployeeAsync(periodId, employeeId);
            if (!existingLines.Any())
                return null;

            var employee = await _employeeReadWrite.ReadEmployeeByIdAsync((int)employeeId);
            return new PaySlipBusinessModel
            {
                BankAccountNumber = employee?.BankAccountNumber,
                Classification = employee?.Classification,
                Dependents = employee?.Dependents ?? 0,
                EmployeeID = employeeId,
                EmployeeName = employee != null ? $"{employee.Prenom} {employee.Nom}" : string.Empty,
                Lines = existingLines.Select(PaySlipLineBusinessModel.FromDataModel).ToList(),
                NumeroCnaps = employee?.NumeroCnaps,
                PayrollID = existingLines.First().PayrollID,
                PeriodID = periodId,
                Poste = employee?.NomPoste
            };
        }

        public async Task SetPeriodStatusAsync(int periodId, string status)
        {
            await _periodReadWrite.UpdatePeriodStatusAsync(periodId, status);
        }

        public async Task SetPaySlipAsync(PaySlipBusinessModel paySlip, DateTime paymentDate, int periodMonth, int periodYear)
        {
            var payrollData = new EmployeePayrollDataModel
            {
                BaseSalary = paySlip.TotalGains,
                Bonus = 0,
                Deductions = paySlip.TotalCotisationsEmployee + paySlip.Irsa,
                EmployeeID = paySlip.EmployeeID,
                NetSalary = paySlip.NetAPayer,
                PayrollID = paySlip.PayrollID,
                PaymentDate = paymentDate,
                PaymentMethod = "BankTransfer",
                PayPeriodMonth = periodMonth,
                PayPeriodYear = periodYear
            };

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
                PayrollID = paySlip.PayrollID,
                PeriodID = l.PeriodID,
                Rubrique = l.Rubrique,
                SortOrder = l.SortOrder,
                Taux = l.Taux
            }).ToList();

            await _paySlipUpdate.UpdatePaySlipAsync(payrollData, lineDataModels);
        }

        public async Task<int> AddModificationRequestAsync(PaySlipModificationRequestBusinessModel request)
        {
            var dataModel = new PaySlipModificationRequestDataModel
            {
                Bonus = request.Bonus,
                Comments = request.Comments,
                EmployeeID = request.EmployeeID,
                IndemniteLogement = request.IndemniteLogement,
                IndemniteTransport = request.IndemniteTransport,
                OvertimeHours = request.OvertimeHours,
                PeriodID = request.PeriodID,
                PrimeScolarite = request.PrimeScolarite,
                Status = "Pending",
                TreiziemeMois = request.TreiziemeMois
            };
            return await _modificationRequestWrite.CreateRequestAsync(dataModel);
        }

        public async Task<PaySlipModificationRequestBusinessModel?> GetModificationRequestAsync(long employeeId, int periodId)
        {
            var data = await _modificationRequestRead.ReadPendingByEmployeeAndPeriodAsync(employeeId, periodId);
            return data != null ? PaySlipModificationRequestBusinessModel.FromDataModel(data) : null;
        }

        public async Task<List<PaySlipModificationRequestBusinessModel>> GetModificationRequestsByPeriodAsync(int periodId)
        {
            var data = await _modificationRequestRead.ReadPendingByPeriodAsync(periodId);
            return data.Select(PaySlipModificationRequestBusinessModel.FromDataModel).ToList();
        }

        public async Task SetModificationRequestStatusAsync(int requestId, string status)
        {
            await _modificationRequestWrite.UpdateRequestStatusAsync(requestId, status);
        }
    }
}
