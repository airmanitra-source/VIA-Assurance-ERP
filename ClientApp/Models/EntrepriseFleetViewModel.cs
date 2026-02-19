using System.ComponentModel.DataAnnotations;
using Company.Fleet.Module.Business;

namespace ClientApp.Models
{
    public class EntrepriseFleetViewModel
    {
        public long EntrepriseId { get; set; }

        public int? FiscalPower { get; set; }

        public long Id { get; set; }

        public string Immatriculation { get; set; } = string.Empty;

        public DateTime? InsuranceEndDate { get; set; }

        public DateTime? InsuranceStartDate { get; set; }

        public bool IsEligibleForInsurance => IsWorking && Year >= (DateTime.Now.Year - 20);

        public bool IsInsured { get; set; }

        public bool IsWorking { get; set; } = true;

        [Required(ErrorMessage = "Make is required")]
        public string Make { get; set; } = string.Empty;

        [Range(0, 10000000, ErrorMessage = "Mileage must be a positive number")]
        public int Mileage { get; set; }

        [Required(ErrorMessage = "Model is required")]
        public string Model { get; set; } = string.Empty;

        public string? PolicyNumber { get; set; }

        public int? Power { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = "Auto"; // 'Auto' or 'Moto'

        public bool WantsInsurance { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2100, ErrorMessage = "Please enter a valid year")]
        public int Year { get; set; } = DateTime.Now.Year;

        internal static List<EntrepriseFleetViewModel>? FromBusinessModel(List<EntrepriseFleetBusinessModel> entrepriseFleetBusinessModels)
        {
           return entrepriseFleetBusinessModels.Select(x => new EntrepriseFleetViewModel
           {
               EntrepriseId = x.EntrepriseId,
               FiscalPower = x.FiscalPower,
               Id = x.Id,
               InsuranceEndDate = x.InsuranceEndDate,
               InsuranceStartDate = x.InsuranceStartDate,
               IsInsured = x.IsInsured,
               IsWorking = x.IsWorking,
               Make = x.Make,
               Mileage = x.Mileage,
               Model = x.Model,
               Power = x.Power,
               Type = x.Type,
               WantsInsurance = x.WantsInsurance,
               Year = x.Year
           }).ToList();
        }
    }
}