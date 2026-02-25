namespace FileTable.Infrastructure.FileTableDb.Entities
{
    internal class EmployeePayrollEntity
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
    }
}
