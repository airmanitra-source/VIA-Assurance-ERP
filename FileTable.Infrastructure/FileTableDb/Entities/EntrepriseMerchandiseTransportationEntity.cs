namespace FileTable.Infrastructure.FileTableDb.Entities
{
    public class EntrepriseMerchandiseTransportationEntity
    {
        public long Id { get; set; }

        public DateTime ArrivalDate { get; set; }

        public DateTime DepartureDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public long EntrepriseId { get; set; }

        public string Frequency { get; set; } = "OneTime";

        public bool IsInsured { get; set; }

        public string Origin { get; set; } = string.Empty;

        public decimal Value { get; set; }

        public bool WantsInsurance { get; set; }
    }
}
