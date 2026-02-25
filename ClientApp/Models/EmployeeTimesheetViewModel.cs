using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class EmployeeTimesheetViewModel
    {
        public long TimesheetID { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public long EmployeeID { get; set; }

        [Range(0.5, 24, ErrorMessage = "Les heures travaillées doivent être entre 0.5 et 24")]
        [Required(ErrorMessage = "Les heures travaillées sont requises")]
        public decimal HoursWorked { get; set; }

        [Range(0, 12, ErrorMessage = "Les heures supplémentaires doivent être entre 0 et 12")]
        public decimal OvertimeHours { get; set; }

        public long? ProjectID { get; set; }
        public string Status { get; set; } = "Draft";

        [MaxLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string? TaskDescription { get; set; }

        [Required(ErrorMessage = "La date de travail est requise")]
        public DateTime WorkDate { get; set; } = DateTime.Today;
    }
}
