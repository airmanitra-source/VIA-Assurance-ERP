using CompanyPayroll.Module.Business;
using Employee.Module.Business.Models;

namespace PaySlip.Module.Business.Models
{
    /// <summary>
    /// Bulletin de paie complet avec calculs IRSA/CNAPS/OSTIE conformes à la législation malgache
    /// </summary>
    public class PaySlipBusinessModel
    {
        public long EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? BankAccountNumber { get; set; }
        public string? Classification { get; set; }
        public int Dependents { get; set; }
        public List<PaySlipLineBusinessModel> Lines { get; set; } = new();
        public string? NumeroCnaps { get; set; }
        public int PayrollID { get; set; }
        public int PeriodID { get; set; }
        public string? Poste { get; set; }

        // --- Totaux calculés ---
        public decimal TotalGains => Lines.Where(l => l.LineType == "Gain").Sum(l => l.GainAmount);
        public decimal TotalCotisationsEmployee => Lines.Where(l => l.LineType == "Cotisation").Sum(l => l.EmployeeDeduction);
        public decimal TotalCotisationsEmployer => Lines.Where(l => l.LineType == "Cotisation").Sum(l => l.EmployerContribution);
        public decimal BaseImposable => TotalGains - TotalCotisationsEmployee;
        public decimal Irsa => Lines.Where(l => l.LineType == "Impot").Sum(l => l.EmployeeDeduction);
        public decimal NetAPayer => TotalGains - TotalCotisationsEmployee - Irsa;

        /// <summary>
        /// Génère les lignes du bulletin de paie à partir des données saisies et des paramètres entreprise
        /// </summary>
        public static PaySlipBusinessModel Generate(
            EmployeeBusinessModel employee,
            CompanyPayrollSettingsBusinessModel settings,
            int periodId,
            int payrollId,
            decimal? bonus = null,
            decimal? primeScolarite = null,
            decimal? treiziemeMois = null,
            decimal? indemniteTransport = null,
            decimal? indemniteLogement = null,
            decimal? overtimeHours = null)
        {
            var slip = new PaySlipBusinessModel
            {
                BankAccountNumber = employee.BankAccountNumber,
                Classification = employee.Classification,
                Dependents = employee.Dependents,
                EmployeeID = employee.EmployeeID,
                EmployeeName = $"{employee.Prenom} {employee.Nom}",
                NumeroCnaps = employee.NumeroCnaps,
                PayrollID = payrollId,
                PeriodID = periodId,
                Poste = employee.NomPoste
            };

            decimal baseSalary = employee.Salaire ?? 0;
            int sortOrder = 0;

            // === GAINS ===
            slip.Lines.Add(new PaySlipLineBusinessModel
            {
                EmployeeID = employee.EmployeeID,
                GainAmount = baseSalary,
                Libelle = "Salaire Fixe Mensuel",
                LineType = "Gain",
                Nombre = 30,
                Base = Math.Round(baseSalary / 30, 2),
                PayrollID = payrollId,
                PeriodID = periodId,
                Rubrique = "6500",
                SortOrder = ++sortOrder
            });

            if (bonus.HasValue && bonus.Value > 0)
            {
                slip.Lines.Add(new PaySlipLineBusinessModel
                {
                    EmployeeID = employee.EmployeeID,
                    GainAmount = bonus.Value,
                    Libelle = "Bonus",
                    LineType = "Gain",
                    PayrollID = payrollId,
                    PeriodID = periodId,
                    Rubrique = "17000",
                    SortOrder = ++sortOrder
                });
            }

            if (primeScolarite.HasValue && primeScolarite.Value > 0)
            {
                slip.Lines.Add(new PaySlipLineBusinessModel
                {
                    EmployeeID = employee.EmployeeID,
                    GainAmount = primeScolarite.Value,
                    Libelle = "Prime de scolarité",
                    LineType = "Gain",
                    PayrollID = payrollId,
                    PeriodID = periodId,
                    Rubrique = "17100",
                    SortOrder = ++sortOrder
                });
            }

            if (treiziemeMois.HasValue && treiziemeMois.Value > 0)
            {
                slip.Lines.Add(new PaySlipLineBusinessModel
                {
                    EmployeeID = employee.EmployeeID,
                    GainAmount = treiziemeMois.Value,
                    Libelle = "13ème mois",
                    LineType = "Gain",
                    PayrollID = payrollId,
                    PeriodID = periodId,
                    Rubrique = "17200",
                    SortOrder = ++sortOrder
                });
            }

            if (indemniteTransport.HasValue && indemniteTransport.Value > 0)
            {
                slip.Lines.Add(new PaySlipLineBusinessModel
                {
                    EmployeeID = employee.EmployeeID,
                    GainAmount = indemniteTransport.Value,
                    Libelle = "Indemnité de déplacement",
                    LineType = "Gain",
                    PayrollID = payrollId,
                    PeriodID = periodId,
                    Rubrique = "19400",
                    SortOrder = ++sortOrder
                });
            }

            if (indemniteLogement.HasValue && indemniteLogement.Value > 0)
            {
                slip.Lines.Add(new PaySlipLineBusinessModel
                {
                    EmployeeID = employee.EmployeeID,
                    GainAmount = indemniteLogement.Value,
                    Libelle = "Indemnité de logement",
                    LineType = "Gain",
                    PayrollID = payrollId,
                    PeriodID = periodId,
                    Rubrique = "19500",
                    SortOrder = ++sortOrder
                });
            }

            if (overtimeHours.HasValue && overtimeHours.Value > 0)
            {
                decimal hourlyRate = baseSalary / 173.33m;
                decimal overtimeAmount = Math.Round(overtimeHours.Value * hourlyRate * settings.OvertimeRateMultiplier, 2);
                slip.Lines.Add(new PaySlipLineBusinessModel
                {
                    Base = Math.Round(hourlyRate * settings.OvertimeRateMultiplier, 2),
                    EmployeeID = employee.EmployeeID,
                    GainAmount = overtimeAmount,
                    Libelle = "Heures supplémentaires",
                    LineType = "Gain",
                    Nombre = overtimeHours.Value,
                    PayrollID = payrollId,
                    PeriodID = periodId,
                    Rubrique = "15000",
                    SortOrder = ++sortOrder
                });
            }

            decimal totalGains = slip.Lines.Sum(l => l.GainAmount);

            // === COTISATIONS ===
            decimal cnapsEmployee = Math.Round(totalGains * settings.CnapsEmployeeRate, 2);
            decimal cnapsEmployer = Math.Round(totalGains * settings.CnapsEmployerRate, 2);
            slip.Lines.Add(new PaySlipLineBusinessModel
            {
                Base = totalGains,
                EmployeeDeduction = cnapsEmployee,
                EmployeeID = employee.EmployeeID,
                EmployerContribution = cnapsEmployer,
                Libelle = "CNaPS",
                LineType = "Cotisation",
                PayrollID = payrollId,
                PeriodID = periodId,
                Rubrique = "40100",
                SortOrder = ++sortOrder,
                Taux = settings.CnapsEmployeeRate
            });

            decimal cnapsCompEmployee = Math.Round(totalGains * settings.CnapsComplementaryRate, 2);
            decimal cnapsCompEmployer = Math.Round(totalGains * settings.CnapsComplementaryEmployerRate, 2);
            slip.Lines.Add(new PaySlipLineBusinessModel
            {
                Base = totalGains,
                EmployeeDeduction = cnapsCompEmployee,
                EmployeeID = employee.EmployeeID,
                EmployerContribution = cnapsCompEmployer,
                Libelle = "CNaPS Complémentaire",
                LineType = "Cotisation",
                PayrollID = payrollId,
                PeriodID = periodId,
                Rubrique = "40400",
                SortOrder = ++sortOrder,
                Taux = settings.CnapsComplementaryRate
            });

            decimal ostieEmployee = Math.Round(totalGains * settings.OstieEmployeeRate, 2);
            decimal ostieEmployer = Math.Round(totalGains * settings.OstieEmployerRate, 2);
            slip.Lines.Add(new PaySlipLineBusinessModel
            {
                Base = totalGains,
                EmployeeDeduction = ostieEmployee,
                EmployeeID = employee.EmployeeID,
                EmployerContribution = ostieEmployer,
                Libelle = "Cotisation OSTIE",
                LineType = "Cotisation",
                PayrollID = payrollId,
                PeriodID = periodId,
                Rubrique = "41000",
                SortOrder = ++sortOrder,
                Taux = settings.OstieEmployeeRate
            });

            // === IRSA ===
            decimal totalCotisationsEmployee = cnapsEmployee + cnapsCompEmployee + ostieEmployee;
            decimal baseImposable = totalGains - totalCotisationsEmployee;
            decimal irsa = CalculateIrsa(baseImposable, employee.Dependents, settings.IrsaDependentReduction, settings.IrsaMinimum);

            slip.Lines.Add(new PaySlipLineBusinessModel
            {
                EmployeeDeduction = irsa,
                EmployeeID = employee.EmployeeID,
                GainAmount = baseImposable,
                Libelle = "Base imposable",
                LineType = "Impot",
                PayrollID = payrollId,
                PeriodID = periodId,
                Rubrique = "66000",
                SortOrder = ++sortOrder
            });

            if (employee.Dependents > 0)
            {
                slip.Lines.Add(new PaySlipLineBusinessModel
                {
                    Base = settings.IrsaDependentReduction,
                    EmployeeID = employee.EmployeeID,
                    GainAmount = employee.Dependents * settings.IrsaDependentReduction,
                    Libelle = "Abattement sur enfant",
                    LineType = "Impot",
                    Nombre = employee.Dependents,
                    PayrollID = payrollId,
                    PeriodID = periodId,
                    Rubrique = "67000",
                    SortOrder = ++sortOrder
                });
            }

            slip.Lines.Add(new PaySlipLineBusinessModel
            {
                EmployeeDeduction = irsa,
                EmployeeID = employee.EmployeeID,
                Libelle = "Prélèvement IRSA",
                LineType = "Impot",
                PayrollID = payrollId,
                PeriodID = periodId,
                Rubrique = "68000",
                SortOrder = ++sortOrder
            });

            return slip;
        }

        /// <summary>
        /// Calcul IRSA selon les tranches malgaches
        /// </summary>
        public static decimal CalculateIrsa(decimal baseImposable, int dependents, decimal dependentReduction, decimal irsaMinimum)
        {
            decimal irsa = 0;

            if (baseImposable <= 350_000)
            {
                irsa = 0;
            }
            else if (baseImposable <= 400_000)
            {
                irsa = (baseImposable - 350_000) * 0.05m;
            }
            else if (baseImposable <= 500_000)
            {
                irsa = (50_000 * 0.05m) + (baseImposable - 400_000) * 0.10m;
            }
            else if (baseImposable <= 600_000)
            {
                irsa = (50_000 * 0.05m) + (100_000 * 0.10m) + (baseImposable - 500_000) * 0.15m;
            }
            else
            {
                irsa = (50_000 * 0.05m) + (100_000 * 0.10m) + (100_000 * 0.15m) + (baseImposable - 600_000) * 0.20m;
            }

            // Réduction pour personnes à charge
            irsa -= dependents * dependentReduction;

            // Minimum IRSA
            if (irsa < irsaMinimum)
                irsa = irsaMinimum;

            return Math.Round(irsa, 2);
        }
    }
}
