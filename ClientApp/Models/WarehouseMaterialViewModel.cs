using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class WarehouseMaterialViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(250, ErrorMessage = "Description is too long")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Approximate value is required")]
        [Range(0, 1000000000, ErrorMessage = "Value must be positive")]
        public decimal ApproximateValue { get; set; }

        public bool WantsInsurance { get; set; }
        public bool IsInsured { get; set; }
    }
}
