namespace FileTable.Infrastructure.FileTableDb.Entities
{
    public class EntrepriseWarehouseEntity
    {
        public string? Address { get; set; }

        public string? ContentsDescription { get; set; }

        public long EntrepriseId { get; set; }

        public long Id { get; set; }

        public DateTime? InsuranceEndDate { get; set; }

        public DateTime? InsuranceStartDate { get; set; }

        public bool IsInsured { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal SizeM2 { get; set; }

        public bool WantsInsurance { get; set; }
        
        public decimal? FranchiseAmount { get; set; }
        
        public decimal? FranchisePercentage { get; set; }
        
        public string FranchiseType { get; set; } = "Fixed";
    }
}
