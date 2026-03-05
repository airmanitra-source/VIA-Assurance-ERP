using CompanyPayroll.Module;
using Employee.Module.Business;
using Employee.Module.Data.Providers;
using EmployeePayroll.Module.Business;
using EmployeePayroll.Module.Data.Models;
using EmployeePayroll.Module.Data.Providers;
using FileTable.Infrastructure.Abstractions;
using PaySlip.Module.Business;
using PaySlip.Module.Data.Models;
using PaySlip.Module.Data.Providers;

namespace PaySlip.Module
{
    public class PaySlipModule : IPaySlipModule
    {
        private readonly ICompanyPayrollModule _companyPayrollModule;
        private readonly IEmployeeReadWrite _employeeReadWrite;
        private readonly IEmployeePayrollReadWrite _payrollReadWrite;
        private readonly IPayrollPeriodReadOnly _periodReadOnly;
        private readonly IPaySlipLineReadOnly _lineReadOnly;
        private readonly IPaySlipLineReadWrite _lineReadWrite;
        private readonly IPaySlipModificationRequestReadOnly _modificationRequestRead;
        private readonly IPaySlipModificationRequestReadWrite _modificationRequestWrite;
        private readonly IPaySlipReadWrite _paySlipUpdate;
        private readonly IPaySlipSecondEntryReadOnly _secondEntryRead;
        private readonly IPaySlipSecondEntryReadWrite _secondEntryWrite;
        private readonly ITransactionHandler _transactionHandler;

        public PaySlipModule(
            ICompanyPayrollModule companyPayrollModule,
            IEmployeeReadWrite employeeReadWrite,
            IEmployeePayrollReadWrite payrollReadWrite,
            IPayrollPeriodReadOnly periodReadOnly,
            IPaySlipLineReadOnly lineReadOnly,
            IPaySlipLineReadWrite lineReadWrite,
            IPaySlipModificationRequestReadOnly modificationRequestRead,
            IPaySlipModificationRequestReadWrite modificationRequestWrite,
            IPaySlipReadWrite paySlipUpdate,
            IPaySlipSecondEntryReadOnly secondEntryRead,
            IPaySlipSecondEntryReadWrite secondEntryWrite,
            ITransactionHandler transactionHandler)
        {
            _companyPayrollModule = companyPayrollModule;
            _employeeReadWrite = employeeReadWrite;
            _payrollReadWrite = payrollReadWrite;
            _periodReadOnly = periodReadOnly;
            _lineReadOnly = lineReadOnly;
            _lineReadWrite = lineReadWrite;
            _modificationRequestRead = modificationRequestRead;
            _modificationRequestWrite = modificationRequestWrite;
            _paySlipUpdate = paySlipUpdate;
            _secondEntryRead = secondEntryRead;
            _secondEntryWrite = secondEntryWrite;
            _transactionHandler = transactionHandler;
        }

        public async Task<int> AddPaySlipAsync(PaySlipBusinessModel paySlip, DateTime paymentDate, int periodMonth, int periodYear)
        {
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

        public async Task<PaySlipBusinessModel> GetPaySlipAsync(long employeeId, int periodId, long enterpriseId)
        {
            var settings = await _companyPayrollModule.GetSettingsAsync(enterpriseId);

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

        public async Task RemovePayrollAsync(int payrollId)
        {
            await _transactionHandler.ExecuteInTransactionAsync(async () =>
            {
                var payrollLines = await _lineReadOnly.ReadByPayrollIdAsync(payrollId);
                if (payrollLines.Any())
                {
                    var firstLine = payrollLines.First();
                    var employeeId = firstLine.EmployeeID;
                    var periodId = firstLine.PeriodID;

                    await _modificationRequestWrite.DeleteByEmployeeAndPeriodAsync(employeeId, periodId);
                }

                await _lineReadWrite.DeleteByPayrollIdAsync(payrollId);
                await _payrollReadWrite.DeletePayrollAsync(payrollId);
                return true;
            });
        }

        public async Task SetRecalculateDraftPaySlipsForEmployeeAsync(long employeeId, long enterpriseId)
        {
            await SetRecalculateDraftPaySlipsForEmployeeAsync(employeeId, enterpriseId, null);
        }

        public async Task SetRecalculateDraftPaySlipsForEmployeeAsync(long employeeId, long enterpriseId, decimal? updatedSalary = null)
        {
            var settings = await _companyPayrollModule.GetSettingsAsync(enterpriseId);
            var periods = await _periodReadOnly.ReadByEnterpriseIdAsync(enterpriseId);
            var draftPeriods = periods
                .Select(PayrollPeriodBusinessModel.FromDataModel)
                .Where(p => p.Status == "Draft" || p.Status == "Open")
                .ToList();

            if (!draftPeriods.Any())
                return;

            var empData = await _employeeReadWrite.ReadEmployeeByIdAsync((int)employeeId);
            if (empData == null)
                return;

            var employee = new EmployeeBusinessModel
            {
                BankAccountNumber = empData.BankAccountNumber,
                Classification = empData.Classification,
                Dependents = empData.Dependents,
                EmployeeID = empData.EmployeeID,
                Nom = empData.Nom,
                NomPoste = empData.NomPoste,
                NumeroCnaps = empData.NumeroCnaps,
                Prenom = empData.Prenom,
                Salaire = updatedSalary ?? empData.Salaire
            };

            await _transactionHandler.ExecuteInTransactionAsync(async () =>
            {
                foreach (var period in draftPeriods)
                {
                    var existingLines = await _lineReadOnly.ReadByPeriodAndEmployeeAsync(period.PeriodID, employeeId);
                    if (!existingLines.Any())
                        continue;

                    int payrollId = existingLines.First().PayrollID;

                    decimal? bonus = existingLines.FirstOrDefault(l => l.Rubrique == "17000")?.GainAmount;
                    decimal? primeScolarite = existingLines.FirstOrDefault(l => l.Rubrique == "17100")?.GainAmount;
                    decimal? treiziemeMois = existingLines.FirstOrDefault(l => l.Rubrique == "17200")?.GainAmount;
                    decimal? indemniteTransport = existingLines.FirstOrDefault(l => l.Rubrique == "19400")?.GainAmount;
                    decimal? indemniteLogement = existingLines.FirstOrDefault(l => l.Rubrique == "19500")?.GainAmount;
                    decimal? overtimeHours = existingLines.FirstOrDefault(l => l.Rubrique == "15000")?.Nombre;

                    var regenerated = PaySlipBusinessModel.Generate(
                        employee,
                        settings,
                        period.PeriodID,
                        payrollId,
                        bonus: bonus,
                        primeScolarite: primeScolarite,
                        treiziemeMois: treiziemeMois,
                        indemniteTransport: indemniteTransport,
                        indemniteLogement: indemniteLogement,
                        overtimeHours: overtimeHours);

                    var payrollData = new EmployeePayrollDataModel
                    {
                        BaseSalary = regenerated.TotalGains,
                        Bonus = 0,
                        Deductions = regenerated.TotalCotisationsEmployee + regenerated.Irsa,
                        EmployeeID = regenerated.EmployeeID,
                        NetSalary = regenerated.NetAPayer,
                        PayrollID = payrollId,
                        PaymentDate = period.PaymentDate ?? DateTime.Now,
                        PaymentMethod = "BankTransfer",
                        PayPeriodMonth = period.PeriodStart.Month,
                        PayPeriodYear = period.PeriodStart.Year
                    };

                    var lineDataModels = regenerated.Lines.Select(l => new PaySlipLineDataModel
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

                    await _paySlipUpdate.UpdatePaySlipAsync(payrollData, lineDataModels);
                }

                return true;
            });
        }

        public async Task<int> AddSecondEntryAsync(PaySlipSecondEntryBusinessModel entry)
        {
            var dataModel = new PaySlipSecondEntryDataModel
            {
                Bonus = entry.Bonus,
                EmployeeID = entry.EmployeeID,
                IndemniteLogement = entry.IndemniteLogement,
                IndemniteTransport = entry.IndemniteTransport,
                OvertimeHours = entry.OvertimeHours,
                PeriodID = entry.PeriodID,
                PrimeScolarite = entry.PrimeScolarite,
                TreiziemeMois = entry.TreiziemeMois
            };
            return await _secondEntryWrite.CreateSecondEntryAsync(dataModel);
        }

        public async Task<PaySlipSecondEntryBusinessModel?> GetSecondEntryAsync(long employeeId, int periodId)
        {
            var data = await _secondEntryRead.ReadByEmployeeAndPeriodAsync(employeeId, periodId);
            return data != null ? PaySlipSecondEntryBusinessModel.FromDataModel(data) : null;
        }

        public async Task RemoveSecondEntryAsync(long employeeId, int periodId)
        {
            await _secondEntryWrite.DeleteByEmployeeAndPeriodAsync(employeeId, periodId);
        }
    }
}
