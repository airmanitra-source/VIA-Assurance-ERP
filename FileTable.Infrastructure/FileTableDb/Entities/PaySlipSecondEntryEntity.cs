namespace FileTable.Infrastructure.FileTableDb.Entities
{
    internal class PaySlipSecondEntryEntity
    {
        public int SecondEntryID { get; set; }
        public decimal? Bonus { get; set; }
        public DateTime CreatedDate { get; set; }
        public long EmployeeID { get; set; }
        public decimal? IndemniteLogement { get; set; }
        public decimal? IndemniteTransport { get; set; }
        public decimal? OvertimeHours { get; set; }
        public int PeriodID { get; set; }
        public decimal? PrimeScolarite { get; set; }
        public decimal? TreiziemeMois { get; set; }
    }
}
