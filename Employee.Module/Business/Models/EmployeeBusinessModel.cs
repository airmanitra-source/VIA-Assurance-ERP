namespace Employee.Module.Business.Models
{
    public class EmployeeBusinessModel
    {
        public long EmployeeID { get; set; }

        public int Age { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? Classification { get; set; }

        public DateTime? DateEmbauche { get; set; }

        public DateTime? DateFinContrat { get; set; }

        public int Dependents { get; set; }

        public string? Email { get; set; }

        public long EntrepriseID { get; set; }

        public string? Fonctions { get; set; }

        public bool IsActive { get; set; } = true;

        public string Nom { get; set; } = string.Empty;

        public int NombreMoisPoste { get; set; }

        public string? NomPoste { get; set; }

        public string? NumeroCnaps { get; set; }

        public string? NumeroMatricule { get; set; }

        public string Prenom { get; set; } = string.Empty;

        public decimal? Salaire { get; set; }

        public string Sexe { get; set; } = string.Empty;

        public string StatutEmploye { get; set; } = string.Empty;

        public bool VouloirSouscrire { get; set; } = false;
    }
}
