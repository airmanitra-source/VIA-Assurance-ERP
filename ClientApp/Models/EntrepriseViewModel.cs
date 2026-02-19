
using Company.Module.Business;

namespace ClientApp.Models
{
    public class EntrepriseViewModel
    {
        public long Id { get; set; }
        public string? RaisonSocial { get; set; }
        public string? StatutJuridique { get; set; }
        public int? Taille { get; set; }
        public int? NombreEmployes { get; set; }
        public string? Adresse { get; set; }
        public DateTime? AnneeCreation { get; set; }
        public string? NifStat { get; set; }
        public string? Email { get; set; }

        internal static EntrepriseViewModel? FromBusinessModel(EntrepriseBusinessModel? companyBusinessModel)
        {
            if (companyBusinessModel == null) return new();
            return new EntrepriseViewModel
            {
                Id = companyBusinessModel.Id,
                RaisonSocial = companyBusinessModel.RaisonSocial,
                StatutJuridique  = companyBusinessModel.StatutJuridique,
                Taille = companyBusinessModel.Taille,
                NombreEmployes = companyBusinessModel.NombreEmployes,
                Adresse = companyBusinessModel.Adresse,
                AnneeCreation = companyBusinessModel.AnneeCreation,
                NifStat = companyBusinessModel.NifStat,
                Email = companyBusinessModel.Email
            };
        }
    }
}