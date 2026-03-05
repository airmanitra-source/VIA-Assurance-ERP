using Employee.Module.Business;
using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class EmployeeViewModel
    {
        public long EmployeeID { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public int Age { get; set; } = 18;

        public DateTime? DateEmbauche { get; set; }

        public DateTime? DateFinContrat { get; set; }

        [EmailAddress(ErrorMessage = "Email must be valid")]
        public string? Email { get; set; }

        public long EntrepriseId { get; set; }

        public string? Fonctions { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Nom est requis")]
        public string Nom { get; set; } = string.Empty;

        [Range(0, 30, ErrorMessage = "Nombre d'enfants doit etre entre 0 et 30")]
        public int NombreEnfants { get; set; }

        [Range(0, 1000, ErrorMessage = "Nombre de mois doit être positif")]
        public int NombreMoisPoste { get; set; }

        public string? NomPoste { get; set; }

        [Required(ErrorMessage = "Matricule est requis")]
        public string NumeroMatricule { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prenom est requis")]
        public string Prenom { get; set; } = string.Empty;

        public int? ProjectID { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salaire must be positive")]
        public decimal? Salaire { get; set; }

        public int? SelectedProjectId { get; set; }

        [Required(ErrorMessage = "Sexe est requis")]
        public string Sexe { get; set; } = "M";

        [Required(ErrorMessage = "Statut est requis")]
        public string StatutEmploye { get; set; } = "CDI";

        public bool VouloirSouscrire { get; set; }

        internal static EmployeeViewModel FromBusinessModel(EmployeeBusinessModel employee)
        {
            return new EmployeeViewModel
            {
                EmployeeID = employee.EmployeeID,
                Age = employee.Age,
                DateEmbauche = employee.DateEmbauche,
                DateFinContrat = employee.DateFinContrat,
                Email = employee.Email,
                Fonctions = employee.Fonctions,
                IsActive = employee.IsActive,
                Nom = employee.Nom,
                NombreEnfants = employee.Dependents,
                NombreMoisPoste = employee.NombreMoisPoste,
                NomPoste = employee.NomPoste,
                NumeroMatricule = employee.NumeroMatricule,
                Prenom = employee.Prenom,
                Salaire = employee.Salaire,
                Sexe = employee.Sexe,
                StatutEmploye = employee.StatutEmploye,
                VouloirSouscrire = employee.VouloirSouscrire
            };
        }
    }
}
