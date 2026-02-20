using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class WarehouseViewModel
    {
        [StringLength(250, ErrorMessage = "Adresse trop longue")]
        public string? Address { get; set; }

        public string? ContentsDescription { get; set; }

        public long EntrepriseId { get; set; }

        public long Id { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Montant de la franchise doit être positif")]
        public decimal? FranchiseAmount { get; set; }

        [Range(0, 100, ErrorMessage = "Pourcentage de la franchise doit être entre 0 et 100")]
        public decimal? FranchisePercentage { get; set; }

        public string FranchiseType { get; set; } = "Fixed";

        public DateTime? InsuranceEndDate { get; set; }

        public DateTime? InsuranceStartDate { get; set; }

        public bool IsInsured { get; set; }

        [Required(ErrorMessage = "Nom requis")]
        [StringLength(100, ErrorMessage = "Name is too long")]
        public string Name { get; set; } = string.Empty;

        public string? PolicyNumber { get; set; }

        [Required(ErrorMessage = "Taille en m2 requis")]
        [Range(0.01, 1000000, ErrorMessage = "Taille doit être supérieur à 0")]
        public decimal SizeM2 { get; set; }

        public bool WantsInsurance { get; set; }
    }
}
