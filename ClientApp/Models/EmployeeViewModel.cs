using Employee.Module.Business;
using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class EmployeeViewModel
    {
        public long EmployeeID { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public int Age { get; set; } = 18;

        public DateTime? DateFinContrat { get; set; }

        [EmailAddress(ErrorMessage = "Email must be valid")]
        public string? Email { get; set; }

        public long EntrepriseId { get; set; }

        public string? Fonctions { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Nom is required")]
        public string Nom { get; set; } = string.Empty;

        [Range(0, 1000, ErrorMessage = "Nombre de mois must be positive")]
        public int NombreMoisPoste { get; set; }

        public string? NomPoste { get; set; }

        [Required(ErrorMessage = "Matricule is required")]
        public string NumeroMatricule { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prenom is required")]
        public string Prenom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sexe is required")]
        public string Sexe { get; set; } = "M";

        [Required(ErrorMessage = "Statut is required")]
        public string StatutEmploye { get; set; } = "CDI";

        public bool VouloirSouscrire { get; set; }

        internal static EmployeeViewModel FromBusinessModel(EmployeeBusinessModel employee)
        {
            return new EmployeeViewModel
            {
                EmployeeID = employee.EmployeeID,
                Age = employee.Age,
                DateFinContrat = employee.DateFinContrat,
                Email = employee.Email,
                Fonctions = employee.Fonctions,
                IsActive = employee.IsActive,
                Nom = employee.Nom,
                NombreMoisPoste = employee.NombreMoisPoste,
                NomPoste = employee.NomPoste,
                NumeroMatricule = employee.NumeroMatricule,
                Prenom = employee.Prenom,
                Sexe = employee.Sexe,
                StatutEmploye = employee.StatutEmploye,
                VouloirSouscrire = employee.VouloirSouscrire
            };
        }
    }
}
