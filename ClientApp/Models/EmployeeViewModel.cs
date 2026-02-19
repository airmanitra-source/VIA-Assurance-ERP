using Employee.Module.Business;
using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class EmployeeViewModel
    {
        [Required(ErrorMessage = "Nom is required")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prenom is required")]
        public string Prenom { get; set; } = string.Empty;

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public int Age { get; set; } = 18;

        [Required(ErrorMessage = "Sexe is required")]
        public string Sexe { get; set; } = "M";

        public string? NomPoste { get; set; }
        public string? Fonctions { get; set; }
        
        [Range(0, 1000, ErrorMessage = "Nombre de mois must be positive")]
        public int NombreMoisPoste { get; set; }

        [Required(ErrorMessage = "Statut is required")]
        public string StatutEmploye { get; set; } = "CDI";

        [Required(ErrorMessage = "Matricule is required")]
        public string NumeroMatricule { get; set; } = string.Empty;

        public DateTime? DateFinContrat { get; set; }

        public bool VouloirSouscrire { get; set; }

        public bool IsActive { get; set; }

        public long EmployeeID { get; set; }

        internal static EmployeeViewModel FromBusinessModel(EmployeeBusinessModel employee)
        {
            return new EmployeeViewModel
            {
                EmployeeID = employee.EmployeeID,
                IsActive = employee.IsActive,
                VouloirSouscrire = employee.VouloirSouscrire,
                DateFinContrat = employee.DateFinContrat,
                NumeroMatricule = employee.NumeroMatricule,
                StatutEmploye = employee.StatutEmploye,
                NombreMoisPoste = employee.NombreMoisPoste,
                Fonctions = employee.Fonctions,
                NomPoste = employee.NomPoste,
                Sexe = employee.Sexe,
                Age = employee.Age,
                Prenom = employee.Prenom,
                Nom = employee.Nom
            };
        }
    }
}
