namespace Subscription.Module.Business
{
    public class SouscriptionBusinessModel
    {
        public long Id { get; set; }
        public int EmployeeId { get; set; }
        public long EntrepriseId { get; set; }
        public int MoisDeCotisation { get; set; }
        public int AnneeCotisation { get; set; }
        public decimal MontantCotisation { get; set; }
    }
}
