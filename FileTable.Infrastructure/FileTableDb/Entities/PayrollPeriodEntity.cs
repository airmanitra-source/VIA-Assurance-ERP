namespace FileTable.Infrastructure.FileTableDb.Entities
{
    internal class PayrollPeriodEntity
    {
        public int PeriodID { get; set; }
        public DateTime CreatedDate { get; set; }
        public long EntrepriseID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime PeriodStart { get; set; }
        public string Status { get; set; } = "Draft";
    }
}
