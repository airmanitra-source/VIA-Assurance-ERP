namespace FileTable.Infrastructure.FileTableDb.Entities
{
    internal class SouscriptionEntity
    {
        public long Id { get; set; }
        public int EmployeeId { get; set; }
        public long EntrepriseId { get; set; }
        public int MoisDeCotisation { get; set; }
        public int AnneeCotisation { get; set; }
        public decimal MontantCotisation { get; set; }
    }
}
