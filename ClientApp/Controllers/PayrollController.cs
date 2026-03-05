using ClientApp.Models;
using CompanyPayroll.Module;
using Employee.Module;
using EmployeePayroll.Module;
using FileTable.Infrastructure.Services;
using PaySlip.Module;
using PaySlip.Module.Business;

namespace ClientApp.Controllers
{
    public class PayrollController
    {
        private readonly ICompanyPayrollModule _companyPayrollModule;
        private readonly IEmailService _emailService;
        private readonly IEmployeeModule _employeeModule;
        private readonly IEmployeePayrollModule _employeePayrollModule;
        private readonly IPaySlipModule _paySlipModule;

        public PayrollController(
            ICompanyPayrollModule companyPayrollModule,
            IEmailService emailService,
            IEmployeeModule employeeModule,
            IEmployeePayrollModule employeePayrollModule,
            IPaySlipModule paySlipModule)
        {
            _companyPayrollModule = companyPayrollModule;
            _emailService = emailService;
            _employeeModule = employeeModule;
            _employeePayrollModule = employeePayrollModule;
            _paySlipModule = paySlipModule;
        }

        /// <summary>
        /// REST: Index - Get all payroll periods for an enterprise
        /// </summary>
        public async Task<List<PayrollPeriodViewModel>> Index(long enterpriseId)
        {
            var periods = await _employeePayrollModule.GetPeriodsByEnterpriseAsync(enterpriseId);
            return periods
                .Select(p => new PayrollPeriodViewModel
                {
                    PaymentDate = p.PaymentDate,
                    PeriodEnd = p.PeriodEnd,
                    PeriodID = p.PeriodID,
                    PeriodStart = p.PeriodStart,
                    Status = p.Status
                })
                .OrderByDescending(p => p.PeriodStart)
                .ToList();
        }

        /// <summary>
        /// REST: Show - Get a payslip for a specific employee and period
        /// </summary>
        public async Task<PaySlipViewModel> Show(long employeeId, int periodId, long enterpriseId)
        {
            var paySlip = await _paySlipModule.GetPaySlipAsync(employeeId, periodId, enterpriseId);
            return MapToViewModel(paySlip);
        }

        /// <summary>
        /// REST: Show - Generate a payslip preview with custom inputs
        /// </summary>
        public async Task<PaySlipViewModel> ShowPreviewAsync(
            long employeeId,
            int periodId,
            long enterpriseId,
            PaySlipInputViewModel input)
        {
            var settings = await _companyPayrollModule.GetSettingsAsync(enterpriseId);
            var employees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(enterpriseId);
            var emp = employees.FirstOrDefault(e => e.EmployeeID == employeeId);

            if (emp == null)
                throw new InvalidOperationException("Employee not found");

            var paySlip = PaySlipBusinessModel.Generate(
                emp,
                settings,
                periodId,
                0,
                bonus: input.Bonus,
                primeScolarite: input.PrimeScolarite,
                treiziemeMois: input.TreiziemeMois,
                indemniteTransport: input.IndemniteTransport,
                indemniteLogement: input.IndemniteLogement,
                overtimeHours: input.OvertimeHours);

            return MapToViewModel(paySlip);
        }

        /// <summary>
        /// REST: Store - Create a new payroll period
        /// </summary>
        public async Task<int> Store(long enterpriseId, DateTime periodStart, DateTime periodEnd)
        {
            return await _employeePayrollModule.AddPeriodAsync(enterpriseId, periodStart, periodEnd);
        }

        /// <summary>
        /// REST: Store - Save a payslip for an employee
        /// </summary>
        public async Task<int> StorePaySlipAsync(
            long employeeId,
            int periodId,
            long enterpriseId,
            PaySlipInputViewModel input)
        {
            var settings = await _companyPayrollModule.GetSettingsAsync(enterpriseId);
            var employees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(enterpriseId);
            var emp = employees.FirstOrDefault(e => e.EmployeeID == employeeId);

            if (emp == null)
                throw new InvalidOperationException("Employee not found");

            var period = (await _employeePayrollModule.GetPeriodsByEnterpriseAsync(enterpriseId))
                .FirstOrDefault(p => p.PeriodID == periodId);

            var paySlip = PaySlipBusinessModel.Generate(
                emp,
                settings,
                periodId,
                0,
                bonus: input.Bonus,
                primeScolarite: input.PrimeScolarite,
                treiziemeMois: input.TreiziemeMois,
                indemniteTransport: input.IndemniteTransport,
                indemniteLogement: input.IndemniteLogement,
                overtimeHours: input.OvertimeHours);

            int periodMonth = period?.PeriodStart.Month ?? DateTime.Now.Month;
            int periodYear = period?.PeriodStart.Year ?? DateTime.Now.Year;
            DateTime paymentDate = period?.PaymentDate ?? DateTime.Now;

            return await _paySlipModule.AddPaySlipAsync(paySlip, paymentDate, periodMonth, periodYear);
        }

        /// <summary>
        /// REST: Store - Update a saved payslip for an employee
        /// </summary>
        public async Task StoreSavedPaySlipAsync(long enterpriseId, int periodId, PaySlipViewModel paySlip)
        {
            var periods = await _employeePayrollModule.GetPeriodsByEnterpriseAsync(enterpriseId);
            var period = periods.FirstOrDefault(p => p.PeriodID == periodId);

            int periodMonth = period?.PeriodStart.Month ?? DateTime.Now.Month;
            int periodYear = period?.PeriodStart.Year ?? DateTime.Now.Year;
            DateTime paymentDate = period?.PaymentDate ?? DateTime.Now;

            var business = new PaySlipBusinessModel
            {
                BankAccountNumber = paySlip.BankAccountNumber,
                Classification = paySlip.Classification,
                Dependents = paySlip.Dependents,
                EmployeeID = paySlip.EmployeeID,
                EmployeeName = paySlip.EmployeeName,
                Lines = paySlip.Lines.Select(l => new PaySlipLineBusinessModel
                {
                    Base = l.Base,
                    EmployeeDeduction = l.EmployeeDeduction,
                    EmployeeID = paySlip.EmployeeID,
                    EmployerContribution = l.EmployerContribution,
                    GainAmount = l.GainAmount,
                    Libelle = l.Libelle,
                    LineType = l.LineType,
                    Nombre = l.Nombre,
                    PayrollID = paySlip.PayrollID,
                    PeriodID = paySlip.PeriodID,
                    Rubrique = l.Rubrique,
                    SortOrder = l.SortOrder,
                    Taux = l.Taux
                }).ToList(),
                NumeroCnaps = paySlip.NumeroCnaps,
                PayrollID = paySlip.PayrollID,
                PeriodID = paySlip.PeriodID,
                Poste = paySlip.Poste
            };

            await _paySlipModule.SetPaySlipAsync(business, paymentDate, periodMonth, periodYear);
        }

        private static PaySlipViewModel MapToViewModel(PaySlipBusinessModel business)
        {
            return new PaySlipViewModel
            {
                BankAccountNumber = business.BankAccountNumber,
                Classification = business.Classification,
                Dependents = business.Dependents,
                EmployeeID = business.EmployeeID,
                EmployeeName = business.EmployeeName,
                Lines = business.Lines.Select(l => new PaySlipLineViewModel
                {
                    Base = l.Base,
                    EmployeeDeduction = l.EmployeeDeduction,
                    EmployerContribution = l.EmployerContribution,
                    GainAmount = l.GainAmount,
                    Libelle = l.Libelle,
                    LineType = l.LineType,
                    Nombre = l.Nombre,
                    Rubrique = l.Rubrique,
                    SortOrder = l.SortOrder,
                    Taux = l.Taux
                }).ToList(),
                NumeroCnaps = business.NumeroCnaps,
                PayrollID = business.PayrollID,
                PeriodID = business.PeriodID,
                Poste = business.Poste
            };
        }

        /// <summary>
        /// Submit the draft payslip to the employee via email
        /// </summary>
        public async Task SubmitDraftToEmployeeAsync(long employeeId, int periodId, long enterpriseId)
        {
            var employees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(enterpriseId);
            var emp = employees.FirstOrDefault(e => e.EmployeeID == employeeId);

            if (emp == null)
                throw new InvalidOperationException("Employé introuvable");

            if (string.IsNullOrWhiteSpace(emp.Email))
                throw new InvalidOperationException($"L'employé {emp.Prenom} {emp.Nom} n'a pas d'adresse email configurée");

            var periods = await _employeePayrollModule.GetPeriodsByEnterpriseAsync(enterpriseId);
            var period = periods.FirstOrDefault(p => p.PeriodID == periodId);
            string periodLabel = period != null
                ? $"du {period.PeriodStart:dd/MM/yyyy} au {period.PeriodEnd:dd/MM/yyyy}"
                : $"Période #{periodId}";

            string portalUrl = "/employee-dashboard";

            await _emailService.SendPaySlipDraftEmailAsync(
                emp.Email,
                $"{emp.Prenom} {emp.Nom}",
                periodLabel,
                portalUrl);
        }

        /// <summary>
        /// REST: Index - Get saved payslips for a specific payroll period
        /// </summary>
        public async Task<List<PaySlipViewModel>> IndexSavedPaySlipsAsync(long enterpriseId, int periodId)
        {
            var employees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(enterpriseId);
            var period = (await _employeePayrollModule.GetPeriodsByEnterpriseAsync(enterpriseId))
                .FirstOrDefault(p => p.PeriodID == periodId);

            string periodLabel = period != null
                ? $"du {period.PeriodStart:dd/MM/yyyy} au {period.PeriodEnd:dd/MM/yyyy}"
                : $"Période #{periodId}";

            var result = new List<PaySlipViewModel>();

            foreach (var employee in employees.Where(e => e.IsActive))
            {
                var paySlip = await _paySlipModule.GetSavedPaySlipAsync(employee.EmployeeID, periodId);
                if (paySlip == null)
                    continue;

                // Check if salary changed - compare with actual salary in payslip line
                var salaryLine = paySlip.Lines.FirstOrDefault(l => l.Rubrique == "6500");
                if (salaryLine != null && salaryLine.GainAmount != (employee.Salaire ?? 0))
                {
                    // Recalculate all draft payslips for this employee
                    await _paySlipModule.SetRecalculateDraftPaySlipsForEmployeeAsync(employee.EmployeeID, enterpriseId);

                    // Reload the payslip to get updated values
                    paySlip = await _paySlipModule.GetSavedPaySlipAsync(employee.EmployeeID, periodId);
                    if (paySlip == null)
                        continue;
                }

                var viewModel = MapToViewModel(paySlip);
                viewModel.PeriodLabel = periodLabel;

                result.Add(viewModel);
            }

            return result
                .OrderBy(p => p.EmployeeName)
                .ToList();
        }

        /// <summary>
        /// REST: Index - Get pending modification requests for a period
        /// </summary>
        public async Task<Dictionary<long, PaySlipModificationRequestViewModel>> IndexModificationRequestsAsync(int periodId, long enterpriseId)
        {
            var requests = await _paySlipModule.GetModificationRequestsByPeriodAsync(periodId);
            var employees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(enterpriseId);

            // Group by EmployeeID and take the latest request (by CreatedDate) for each employee
            return requests
                .GroupBy(r => r.EmployeeID)
                .Select(g => g.OrderByDescending(r => r.CreatedDate).First())
                .ToDictionary(
                    r => r.EmployeeID,
                    r =>
                    {
                        var emp = employees.FirstOrDefault(e => e.EmployeeID == r.EmployeeID);
                        return new PaySlipModificationRequestViewModel
                        {
                            Bonus = r.Bonus,
                            Comments = r.Comments,
                            CreatedDate = r.CreatedDate,
                            EmployeeID = r.EmployeeID,
                            EmployeeName = emp != null ? $"{emp.Prenom} {emp.Nom}" : string.Empty,
                            IndemniteLogement = r.IndemniteLogement,
                            IndemniteTransport = r.IndemniteTransport,
                            OvertimeHours = r.OvertimeHours,
                            PeriodID = r.PeriodID,
                            PrimeScolarite = r.PrimeScolarite,
                            RequestID = r.RequestID,
                            Status = r.Status,
                            TreiziemeMois = r.TreiziemeMois
                        };
                    });
        }

        /// <summary>
        /// REST: Destroy - Delete a payslip and its associated lines
        /// </summary>
        public async Task RemovePaySlipAsync(int payrollId)
        {
            await _paySlipModule.RemovePayrollAsync(payrollId);
        }

        /// <summary>
        /// REST: Store - Save the second entry for double-entry validation
        /// </summary>
        public async Task StoreSecondEntryAsync(long employeeId, int periodId, PaySlipInputViewModel input)
        {
            var business = new PaySlipSecondEntryBusinessModel
            {
                Bonus = input.Bonus,
                EmployeeID = employeeId,
                IndemniteLogement = input.IndemniteLogement,
                IndemniteTransport = input.IndemniteTransport,
                OvertimeHours = input.OvertimeHours,
                PeriodID = periodId,
                PrimeScolarite = input.PrimeScolarite,
                TreiziemeMois = input.TreiziemeMois
            };
            await _paySlipModule.AddSecondEntryAsync(business);
        }
    }
}
