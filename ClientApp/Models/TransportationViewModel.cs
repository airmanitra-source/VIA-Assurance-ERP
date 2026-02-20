using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class TransportationViewModel
    {
        [Required(ErrorMessage = "Date d'arrivée requise")]
        public DateTime ArrivalDate { get; set; } = DateTime.Today.AddDays(7);

        [Required(ErrorMessage = "Date de départ requise")]
        public DateTime DepartureDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Description requise")]
        [StringLength(250, ErrorMessage = "Description trop longue")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination is required")]
        public string Destination { get; set; } = string.Empty;

        public long EntrepriseId { get; set; }

        [Required(ErrorMessage = "Frequency is required")]
        public string Frequency { get; set; } = "OneTime";

        public long Id { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Montant de la franchise doit être positif")]
        public decimal? FranchiseAmount { get; set; }

        [Range(0, 100, ErrorMessage = "Franchise percentage must be between 0 and 100")]
        public decimal? FranchisePercentage { get; set; }

        public string FranchiseType { get; set; } = "Fixed";

        public DateTime? InsuranceEndDate { get; set; }

        public DateTime? InsuranceStartDate { get; set; }

        public bool IsInsured { get; set; }

        [Required(ErrorMessage = "Origin is required")]
        public string Origin { get; set; } = string.Empty;

        public string? PolicyNumber { get; set; }

        [Required(ErrorMessage = "Valeur estimée de la marchandise requise")]
        [Range(200000, 1000000000, ErrorMessage = "Valeur estimée de la marchandise doit être positif  et au moins 200 000 MGA")]
        public decimal Value { get; set; }

        public bool WantsInsurance { get; set; }
    }
}
