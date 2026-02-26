namespace Employee.Module.Data.Models
{
    public class EmployeeDataModel
    {
        public long EmployeeID { get; set; }
        public int Age { get; set; }
        public DateTime? DateEmbauche { get; set; }
        public DateTime? DateFinContrat { get; set; }
        public string? Email { get; set; }
        public long EntrepriseID { get; set; }
        public string? Fonctions { get; set; }
        public bool IsActive { get; set; } = true;
        public int NombreMoisPoste { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? NomPoste { get; set; }
        public string Prenom { get; set; } = string.Empty;
        public string? NumeroMatricule { get; set; }
        public decimal? Salaire { get; set; }
        public string Sexe { get; set; } = string.Empty;
        public string StatutEmploye { get; set; } = string.Empty;
        public bool VouloirSouscrire { get; set; } = false;
    }
}
