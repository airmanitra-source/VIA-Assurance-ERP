using ClientApp.Models;
using Employee.Module;
using Employee.Module.Business;
using FileTable.Infrastructure.Services;

namespace ClientApp.Controllers
{
    public class PayrollController
    {
        private readonly IEmailService _emailService;
        private readonly IEmployeeModule _employeeModule;
        private readonly IPayrollModule _payrollModule;

        public PayrollController(
            IEmailService emailService,
            IEmployeeModule employeeModule,
            IPayrollModule payrollModule)
        {
            _emailService = emailService;
            _employeeModule = employeeModule;
            _payrollModule = payrollModule;
        }

        /// <summary>
        /// REST: Index - Get all payroll periods for an enterprise
        /// </summary>
        public async Task<List<PayrollPeriodViewModel>> Index(long enterpriseId)
        {
            var periods = await _payrollModule.GetPeriodsByEnterpriseAsync(enterpriseId);
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
            var paySlip = await _payrollModule.GetPaySlipAsync(employeeId, periodId, enterpriseId);
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
            var settings = await _payrollModule.GetSettingsAsync(enterpriseId);
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
            return await _payrollModule.AddPeriodAsync(enterpriseId, periodStart, periodEnd);
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
            var settings = await _payrollModule.GetSettingsAsync(enterpriseId);
            var employees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(enterpriseId);
            var emp = employees.FirstOrDefault(e => e.EmployeeID == employeeId);

            if (emp == null)
                throw new InvalidOperationException("Employee not found");

            var period = (await _payrollModule.GetPeriodsByEnterpriseAsync(enterpriseId))
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

            return await _payrollModule.AddPaySlipAsync(paySlip, paymentDate, periodMonth, periodYear);
        }

        /// <summary>
        /// REST: Store - Update a saved payslip for an employee
        /// </summary>
        public async Task StoreSavedPaySlipAsync(long enterpriseId, int periodId, PaySlipViewModel paySlip)
        {
            var periods = await _payrollModule.GetPeriodsByEnterpriseAsync(enterpriseId);
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

            await _payrollModule.SetPaySlipAsync(business, paymentDate, periodMonth, periodYear);
        }

        /// <summary>
        /// Get all employees for the payroll period selector
        /// </summary>
        public async Task<List<EmployeeViewModel>> IndexEmployeesAsync(long enterpriseId)
        {
            var employees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(enterpriseId);
            return employees
                .Where(e => e.IsActive)
                .Select(e => new EmployeeViewModel
                {
                    EmployeeID = e.EmployeeID,
                    Nom = e.Nom,
                    NomPoste = e.NomPoste,
                    Prenom = e.Prenom,
                    Salaire = e.Salaire
                })
                .OrderBy(e => e.Nom)
                .ThenBy(e => e.Prenom)
                .ToList();
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

            var periods = await _payrollModule.GetPeriodsByEnterpriseAsync(enterpriseId);
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
            var period = (await _payrollModule.GetPeriodsByEnterpriseAsync(enterpriseId))
                .FirstOrDefault(p => p.PeriodID == periodId);

            string periodLabel = period != null
                ? $"du {period.PeriodStart:dd/MM/yyyy} au {period.PeriodEnd:dd/MM/yyyy}"
                : $"Période #{periodId}";

            var result = new List<PaySlipViewModel>();

            foreach (var employee in employees.Where(e => e.IsActive))
            {
                var paySlip = await _payrollModule.GetSavedPaySlipAsync(employee.EmployeeID, periodId);
                if (paySlip == null)
                    continue;

                var viewModel = MapToViewModel(paySlip);
                viewModel.PeriodLabel = periodLabel;
                result.Add(viewModel);
            }

            return result
                .OrderBy(p => p.EmployeeName)
                .ToList();
        }
    }
}
