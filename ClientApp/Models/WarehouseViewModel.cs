using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class WarehouseViewModel
    {
        public long Id { get; set; }

        [StringLength(250, ErrorMessage = "Address is too long")]
        public string? Address { get; set; }

        public string? ContentsDescription { get; set; }

        public long EntrepriseId { get; set; }

        public bool IsInsured { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name is too long")]
        public string Name { get; set; } = string.Empty;

        public string? PolicyNumber { get; set; }

        [Required(ErrorMessage = "Size is required")]
        [Range(0.01, 1000000, ErrorMessage = "Size must be greater than 0")]
        public decimal SizeM2 { get; set; }

        public bool WantsInsurance { get; set; }
    }
}
