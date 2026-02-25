namespace Employee.Module.Business
{
    public class EmployeePayrollBusinessModel
    {
        public long PayrollID { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Deductions { get; set; }
        public long EmployeeID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal NetSalary { get; set; }
        public string? Notes { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = "BankTransfer";
        public int PayPeriodMonth { get; set; }
        public int PayPeriodYear { get; set; }

        internal static EmployeePayrollBusinessModel FromDataModel(Data.Models.EmployeePayrollDataModel d)
        {
            return new EmployeePayrollBusinessModel
            {
                BaseSalary = d.BaseSalary,
                Bonus = d.Bonus,
                CreatedDate = d.CreatedDate,
                Deductions = d.Deductions,
                EmployeeID = d.EmployeeID,
                ModifiedDate = d.ModifiedDate,
                NetSalary = d.NetSalary,
                Notes = d.Notes,
                PaymentDate = d.PaymentDate,
                PaymentMethod = d.PaymentMethod,
                PayPeriodMonth = d.PayPeriodMonth,
                PayPeriodYear = d.PayPeriodYear,
                PayrollID = d.PayrollID
            };
        }
    }
}
