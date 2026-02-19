namespace ClientApp.Models
{
    public class SubscriptionViewModel
    {
        public long Id { get; set; }

        public int AnneeCotisation { get; set; }

        public long EmployeeId { get; set; }

        public long EntrepriseId { get; set; }

        public int MoisDeCotisation { get; set; }

        public decimal MontantCotisation { get; set; }
    }
}
