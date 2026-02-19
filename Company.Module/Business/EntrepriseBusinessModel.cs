namespace Company.Module.Business
{
    public class EntrepriseBusinessModel
    {
        public long Id { get; set; }

        public string? Adresse { get; set; }

        public DateTime? AnneeCreation { get; set; }

        public string? Email { get; set; }

        public string? NifStat { get; set; }

        public int? NombreEmployes { get; set; }

        public string? RaisonSocial { get; set; }

        public string? StatutJuridique { get; set; }

        public int? Taille { get; set; }
    }
}
