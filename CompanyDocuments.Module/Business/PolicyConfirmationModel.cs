using System;
using System.Collections.Generic;

namespace CompanyDocuments.Module.Business
{
    public class PolicyConfirmationModel
    {
        public string? Address { get; set; }
        public List<CoverageModel> Coverages { get; set; } = new();
        public DateTime? EndDate { get; set; }
        public string? InsuredName { get; set; }
        public string? LicensePlate { get; set; }
        public List<CoverageModel> PolicyCoverages { get; set; } = new();
        public string? PolicyNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public string Title { get; set; } = "CONFIRMATION D'ASSURANCE";
        public List<CoverageModel> VehicleCoverages { get; set; } = new();
        public string? VehicleDescription { get; set; }
        public string? VIN { get; set; }
    }

    public class CoverageModel
    {
        public decimal Amount { get; set; }
        public decimal Deductible { get; set; }
        public string? DeductibleDisplay { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
