namespace FileTable.Infrastructure.FileTableDb.Entities
{
    public class EntrepriseMerchandiseTransportationEntity
    {
        public DateTime ArrivalDate { get; set; }

        public DateTime DepartureDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public long EntrepriseId { get; set; }

        public decimal? FranchiseAmount { get; set; }

        public decimal? FranchisePercentage { get; set; }

        public string FranchiseType { get; set; } = "Fixed";

        public string Frequency { get; set; } = "OneTime";

        public long Id { get; set; }

        public DateTime? InsuranceEndDate { get; set; }

        public DateTime? InsuranceStartDate { get; set; }

        public bool IsInsured { get; set; }

        public string Origin { get; set; } = string.Empty;

        public decimal Value { get; set; }

        public bool WantsInsurance { get; set; }
    }
}
