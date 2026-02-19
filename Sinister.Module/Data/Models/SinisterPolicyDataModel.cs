namespace Sinister.Module.Data.Models
{
    public class SinisterPolicyDataModel
    {
        public long Id { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public long EntrepriseId { get; set; }
        public string? InsurerName { get; set; }
        public string? InsurerContact { get; set; }
        public DateTime CoverageStartDate { get; set; }
        public DateTime? CoverageEndDate { get; set; }
        public string? CoverageType { get; set; }
        public string? CoveredAssets { get; set; }
        public decimal? PremiumAmount { get; set; }
        public decimal? DeductibleAmount { get; set; }
        public string? PolicyLimits { get; set; }
        public bool IsActive { get; set; } = true;
        public string? PolicyReferenceId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
