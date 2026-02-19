using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class TransportationViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(250, ErrorMessage = "Description is too long")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Value of merchandise is required")]
        [Range(200000, 1000000000, ErrorMessage = "Value of merchandise must be positive  and at least 200 000 MGA")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Departure date is required")]
        public DateTime DepartureDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Arrival date is required")]
        public DateTime ArrivalDate { get; set; } = DateTime.Today.AddDays(7);

        [Required(ErrorMessage = "Origin is required")]
        public string Origin { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination is required")]
        public string Destination { get; set; } = string.Empty;

        [Required(ErrorMessage = "Frequency is required")]
        public string Frequency { get; set; } = "OneTime";

        public bool WantsInsurance { get; set; }
        public bool IsInsured { get; set; }
        public string? PolicyNumber { get; set; }
    }
}
