using Company.Module.Business;
using Company.Module.Data.Models;
using Company.Module.Data.Providers;

namespace Company.Module
{
    public class CompanyModule : ICompanyModule
    {
        private readonly IEntrepriseReadOnlyDataProvider _entrepriseReadOnly;

        public CompanyModule(IEntrepriseReadOnlyDataProvider entrepriseReadOnly)
        {
            _entrepriseReadOnly = entrepriseReadOnly;
        }

        public async Task<EntrepriseBusinessModel?> GetCompanyByEmailAsync(string email)
        {
            var dataModel = await _entrepriseReadOnly.ReadEntrepriseByEmailAsync(email);
            return dataModel != null ? MapToBusinessModel(dataModel) : null;
        }

        public async Task<EntrepriseBusinessModel?> GetCompanyByIdAsync(long id)
        {
            var dataModel = await _entrepriseReadOnly.ReadEntrepriseByIdAsync(id);
            return dataModel != null ? MapToBusinessModel(dataModel) : null;
        }

        private static EntrepriseBusinessModel MapToBusinessModel(EntrepriseDataModel dataModel)
        {
            return new EntrepriseBusinessModel
            {
                Id = dataModel.Id,
                RaisonSocial = dataModel.RaisonSocial,
                StatutJuridique = dataModel.StatutJuridique,
                Taille = dataModel.Taille,
                NombreEmployes = dataModel.NombreEmployes,
                Adresse = dataModel.Adresse,
                AnneeCreation = dataModel.AnneeCreation,
                NifStat = dataModel.NifStat,
                Email = dataModel.Email
            };
        }
    }
}

