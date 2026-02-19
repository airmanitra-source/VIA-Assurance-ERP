namespace CompanyDocuments.Module.Business
{
    public class PolicyPdfModel
    {
        public string? PolicyNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? InsuredName { get; set; }
        public string? Address { get; set; }
        public string? VehicleDescription { get; set; }
        public string? VIN { get; set; }
        public List<CoverageModel> Coverages { get; set; } = new();
        public List<CoverageModel> VehicleCoverages { get; set; } = new();
        public List<CoverageModel> PolicyCoverages { get; set; } = new();
    }

    public class CoverageModel
    {
        public string Description { get; set; } = string.Empty;
        public decimal Deductible { get; set; }
        public decimal Amount { get; set; }
    }
}
