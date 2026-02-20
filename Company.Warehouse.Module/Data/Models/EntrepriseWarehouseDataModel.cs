namespace Company.Warehouse.Module.Data.Models
{
    public class EntrepriseWarehouseDataModel
    {
        public string? Address { get; set; }
        public string? ContentsDescription { get; set; }
        public long EntrepriseId { get; set; }
        public decimal? FranchiseAmount { get; set; }
        public decimal? FranchisePercentage { get; set; }
        public string FranchiseType { get; set; } = "Fixed";
        public long Id { get; set; }
        public DateTime? InsuranceEndDate { get; set; }
        public DateTime? InsuranceStartDate { get; set; }
        public bool IsInsured { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PolicyNumber { get; set; }
        public decimal SizeM2 { get; set; }
        public bool WantsInsurance { get; set; }
    }
}
