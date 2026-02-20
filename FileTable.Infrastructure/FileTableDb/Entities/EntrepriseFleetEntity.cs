namespace FileTable.Infrastructure.FileTableDb.Entities
{
    public class EntrepriseFleetEntity
    {
        public long Id { get; set; }

        public long EntrepriseId { get; set; }

        public decimal? FranchiseAmount { get; set; }

        public decimal? FranchisePercentage { get; set; }

        public string FranchiseType { get; set; } = "Fixed";

        public int? FiscalPower { get; set; }


        public DateTime? InsuranceEndDate { get; set; }

        public DateTime? InsuranceStartDate { get; set; }

        public bool IsInsured { get; set; }

        public bool IsWorking { get; set; }

        public string Make { get; set; } = string.Empty;

        public int Mileage { get; set; }

        public string Model { get; set; } = string.Empty;

        public int? Power { get; set; }

        public string Type { get; set; } = string.Empty;

        public bool WantsInsurance { get; set; }

        public int Year { get; set; }
    }
}
