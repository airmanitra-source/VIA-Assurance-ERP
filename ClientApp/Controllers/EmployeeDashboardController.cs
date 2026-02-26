using ClientApp.Models;
using Employee.Module;
using Employee.Module.Business;

namespace ClientApp.Controllers
{
    public class EmployeeDashboardController
    {
        private readonly IEmployeeModule _employeeModule;
        private readonly IEmployeePayrollModule _payrollModule;
        private readonly IPayrollModule _paySlipModule;
        private readonly IEmployeeTimesheetModule _timesheetModule;

        public EmployeeDashboardController(
            IEmployeeModule employeeModule,
            IEmployeePayrollModule payrollModule,
            IPayrollModule paySlipModule,
            IEmployeeTimesheetModule timesheetModule)
        {
            _employeeModule = employeeModule;
            _payrollModule = payrollModule;
            _paySlipModule = paySlipModule;
            _timesheetModule = timesheetModule;
        }

        public async Task<EmployeeViewModel?> ShowProfileAsync(string email)
        {
            var employee = await _employeeModule.GetEmployeeByEmailAsync(email);
            return employee != null ? MapToViewModel(employee) : null;
        }

        public async Task<List<EmployeePayrollViewModel>> IndexPayrollAsync(long employeeId)
        {
            var items = await _payrollModule.GetLastMonthsPayrollAsync(employeeId, 3);
            return items.Select(MapPayrollToViewModel).ToList();
        }

        public async Task<List<PaySlipViewModel>> IndexPaySlipsAsync(long employeeId, long enterpriseId)
        {
            var periods = await _paySlipModule.GetPeriodsByEnterpriseAsync(enterpriseId);
            var recentPeriods = periods.OrderByDescending(p => p.PeriodStart).Take(3).ToList();
            var result = new List<PaySlipViewModel>();

            foreach (var period in recentPeriods)
            {
                try
                {
                    var paySlip = await _paySlipModule.GetPaySlipAsync(employeeId, period.PeriodID, enterpriseId);
                    if (paySlip.Lines.Any())
                    {
                        result.Add(new PaySlipViewModel
                        {
                            BankAccountNumber = paySlip.BankAccountNumber,
                            Classification = paySlip.Classification,
                            Dependents = paySlip.Dependents,
                            EmployeeID = paySlip.EmployeeID,
                            EmployeeName = paySlip.EmployeeName,
                            Lines = paySlip.Lines.Select(l => new PaySlipLineViewModel
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
                            NumeroCnaps = paySlip.NumeroCnaps,
                            PayrollID = paySlip.PayrollID,
                            PeriodID = paySlip.PeriodID,
                            PeriodLabel = $"du {period.PeriodStart:dd/MM/yyyy} au {period.PeriodEnd:dd/MM/yyyy}",
                            Poste = paySlip.Poste
                        });
                    }
                }
                catch { }
            }

            return result;
        }

        public async Task<List<EmployeeTimesheetViewModel>> IndexTimesheetAsync(long employeeId)
        {
            var items = await _timesheetModule.GetTimesheetsByEmployeeIdAsync(employeeId);
            return items.Select(MapTimesheetToViewModel).ToList();
        }

        public async Task<(bool Success, string Message)> StoreTimesheetAsync(EmployeeTimesheetViewModel viewModel, long employeeId)
        {
            try
            {
                var businessModel = new EmployeeTimesheetBusinessModel
                {
                    Comments = viewModel.Comments,
                    EmployeeID = employeeId,
                    HoursWorked = viewModel.HoursWorked,
                    OvertimeHours = viewModel.OvertimeHours,
                    ProjectID = viewModel.ProjectID,
                    Status = "Submitted",
                    TaskDescription = viewModel.TaskDescription,
                    WorkDate = viewModel.WorkDate
                };

                await _timesheetModule.AddTimesheetAsync(businessModel);
                return (true, "Ligne de temps ajoutée avec succès.");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur lors de l'enregistrement : {ex.Message}");
            }
        }

        private static EmployeeViewModel MapToViewModel(EmployeeBusinessModel m)
        {
            return new EmployeeViewModel
            {
                Age = m.Age,
                DateFinContrat = m.DateFinContrat,
                Email = m.Email,
                EmployeeID = m.EmployeeID,
                EntrepriseId = m.EntrepriseID,
                Fonctions = m.Fonctions,
                IsActive = m.IsActive,
                Nom = m.Nom,
                NombreMoisPoste = m.NombreMoisPoste,
                NomPoste = m.NomPoste,
                NumeroMatricule = m.NumeroMatricule ?? string.Empty,
                Prenom = m.Prenom,
                Sexe = m.Sexe,
                StatutEmploye = m.StatutEmploye,
                VouloirSouscrire = m.VouloirSouscrire
            };
        }

        private static EmployeePayrollViewModel MapPayrollToViewModel(EmployeePayrollBusinessModel m)
        {
            return new EmployeePayrollViewModel
            {
                BaseSalary = m.BaseSalary,
                Bonus = m.Bonus,
                CreatedDate = m.CreatedDate,
                Deductions = m.Deductions,
                EmployeeID = m.EmployeeID,
                NetSalary = m.NetSalary,
                Notes = m.Notes,
                PaymentDate = m.PaymentDate,
                PaymentMethod = m.PaymentMethod,
                PayPeriodMonth = m.PayPeriodMonth,
                PayPeriodYear = m.PayPeriodYear,
                PayrollID = m.PayrollID
            };
        }

        private static EmployeeTimesheetViewModel MapTimesheetToViewModel(EmployeeTimesheetBusinessModel m)
        {
            return new EmployeeTimesheetViewModel
            {
                Comments = m.Comments,
                CreatedDate = m.CreatedDate,
                EmployeeID = m.EmployeeID,
                HoursWorked = m.HoursWorked,
                OvertimeHours = m.OvertimeHours,
                ProjectID = m.ProjectID,
                Status = m.Status,
                TaskDescription = m.TaskDescription,
                TimesheetID = m.TimesheetID,
                WorkDate = m.WorkDate
            };
        }
    }
}
