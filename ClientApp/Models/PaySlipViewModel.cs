namespace ClientApp.Models
{
    public class PaySlipViewModel
    {
        public long EmployeeID { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? Classification { get; set; }
        public int Dependents { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public bool IsInvalidated { get; set; }
        public List<PaySlipLineViewModel> Lines { get; set; } = new();
        public string? NumeroCnaps { get; set; }
        public int PayrollID { get; set; }
        public string? PeriodLabel { get; set; }
        public int PeriodID { get; set; }
        public string? Poste { get; set; }

        // Totaux calculés
        public decimal BaseImposable => TotalGains - TotalCotisationsEmployee;
        public decimal Irsa => Lines.Where(l => l.LineType == "Impot" && l.Rubrique == "68000").Sum(l => l.EmployeeDeduction);
        public decimal NetAPayer => TotalGains - TotalCotisationsEmployee - Irsa;
        public decimal TotalCotisationsEmployee => Lines.Where(l => l.LineType == "Cotisation").Sum(l => l.EmployeeDeduction);
        public decimal TotalCotisationsEmployer => Lines.Where(l => l.LineType == "Cotisation").Sum(l => l.EmployerContribution);
        public decimal TotalGains => Lines.Where(l => l.LineType == "Gain").Sum(l => l.GainAmount);
    }
}
