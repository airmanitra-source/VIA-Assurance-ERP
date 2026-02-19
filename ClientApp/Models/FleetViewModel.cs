using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class FleetViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = "Auto"; // Default to Auto

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2100, ErrorMessage = "Please enter a valid year")]
        public int Year { get; set; } = DateTime.Now.Year;

        public bool IsWorking { get; set; } = true;

        [Range(0, 10000000, ErrorMessage = "Mileage must be a positive number")]
        public int Mileage { get; set; }

        [Required(ErrorMessage = "Make is required")]
        public string Make { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        public string Model { get; set; } = string.Empty;

        public bool WantsInsurance { get; set; }
        public bool IsInsured { get; set; }
        public string? PolicyNumber { get; set; }

        // Logic for UI feedback
        public bool IsEligibleForInsurance => IsWorking && Year >= (DateTime.Now.Year - 20);
    }
}
