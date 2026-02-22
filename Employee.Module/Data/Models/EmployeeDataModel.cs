namespace Employee.Module.Data.Models
{
    public class EmployeeDataModel
    {
        public long EmployeeID { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Sexe { get; set; } = string.Empty;
        public string? NomPoste { get; set; }
        public string? Fonctions { get; set; }
        public int NombreMoisPoste { get; set; }
        public string StatutEmploye { get; set; } = string.Empty;
        public long EntrepriseID { get; set; }
        public bool IsActive { get; set; } = true;
        public string? NumeroMatricule { get; set; }
        public DateTime? DateFinContrat { get; set; }
        public string? Email { get; set; }
        public bool VouloirSouscrire { get; set; } = false;
    }
}
