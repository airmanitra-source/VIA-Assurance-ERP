using System.ComponentModel.DataAnnotations;
using Company.Fleet.Module.Business;

namespace ClientApp.Models
{
    public class EntrepriseFleetViewModel
    {
        public long EntrepriseId { get; set; }

        public int? FiscalPower { get; set; }

        public long Id { get; set; }

        public decimal? FranchiseAmount { get; set; }

        public decimal? FranchisePercentage { get; set; }

        public string FranchiseType { get; set; } = "Fixed";

        public string Immatriculation { get; set; } = string.Empty;

        public DateTime? InsuranceEndDate { get; set; }

        public DateTime? InsuranceStartDate { get; set; }

        public bool IsEligibleForInsurance => IsWorking && Year >= (DateTime.Now.Year - 20);

        public bool IsInsured { get; set; }

        public bool IsWorking { get; set; } = true;

        [StringLength(20, ErrorMessage = "La plaque d'immatriculation ne peut pas dépasser 20 caractères")]
        public string? LicensePlate { get; set; }

        [Required(ErrorMessage = "Marque de voiture requise")]
        public string Make { get; set; } = string.Empty;

        [Range(0, 10000000, ErrorMessage = "Kilométrage doit être positif")]
        public int Mileage { get; set; }

        [Required(ErrorMessage = "Modèle est requis")]
        public string Model { get; set; } = string.Empty;

        public string? PolicyNumber { get; set; }

        public int? Power { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = "Auto"; // 'Auto' or 'Moto'

        [StringLength(17, MinimumLength = 17, ErrorMessage = "Le numéro VIN doit contenir exactement 17 caractères")]
        [RegularExpression("^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "Le numéro VIN est invalide (lettres A-Z sauf I, O, Q et chiffres 0-9)")]
        public string? VIN { get; set; }

        public bool WantsInsurance { get; set; }

        [Required(ErrorMessage = "Année de mise en circulation requise")]
        [Range(1900, 2100, ErrorMessage = "L'année doit être valide")]
        public int Year { get; set; } = DateTime.Now.Year;

        internal static List<EntrepriseFleetViewModel>? FromBusinessModel(List<EntrepriseFleetBusinessModel> entrepriseFleetBusinessModels)
        {
           return entrepriseFleetBusinessModels.Select(x => new EntrepriseFleetViewModel
           {
               EntrepriseId = x.EntrepriseId,
               FranchiseAmount = x.FranchiseAmount,
               FranchisePercentage = x.FranchisePercentage,
               FranchiseType = x.FranchiseType,
               FiscalPower = x.FiscalPower,
               Id = x.Id,
               InsuranceEndDate = x.InsuranceEndDate,
               InsuranceStartDate = x.InsuranceStartDate,
               IsInsured = x.IsInsured,
               IsWorking = x.IsWorking,
               LicensePlate = x.LicensePlate,
               Make = x.Make,
               Mileage = x.Mileage,
               Model = x.Model,
               Power = x.Power,
               Type = x.Type,
               VIN = x.VIN,
               WantsInsurance = x.WantsInsurance,
               Year = x.Year
           }).ToList();
        }
    }
}