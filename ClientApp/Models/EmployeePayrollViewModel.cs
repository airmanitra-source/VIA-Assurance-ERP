namespace ClientApp.Models
{
    public class EmployeePayrollViewModel
    {
        public long PayrollID { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Deductions { get; set; }
        public long EmployeeID { get; set; }
        public decimal NetSalary { get; set; }
        public string? Notes { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = "BankTransfer";
        public string PeriodLabel => $"{MonthName(PayPeriodMonth)} {PayPeriodYear}";
        public int PayPeriodMonth { get; set; }
        public int PayPeriodYear { get; set; }

        private static string MonthName(int month) => month switch
        {
            1 => "Janvier", 2 => "Février", 3 => "Mars", 4 => "Avril",
            5 => "Mai", 6 => "Juin", 7 => "Juillet", 8 => "Août",
            9 => "Septembre", 10 => "Octobre", 11 => "Novembre", 12 => "Décembre",
            _ => month.ToString()
        };
    }
}
