using Dapper;
using Company.Module.Data.Models;
using Company.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EntrepriseReadOnly : IEntrepriseReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public EntrepriseReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EntrepriseDataModel?> ReadEntrepriseByIdAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Entreprise] WHERE Id = @Id";
            var entity = await connection.QueryFirstOrDefaultAsync<EntrepriseEntity>(sql, new { Id = id });
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<EntrepriseDataModel?> ReadEntrepriseByEmailAsync(string email)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Entreprise] WHERE Email = @Email";
            var entity = await connection.QueryFirstOrDefaultAsync<EntrepriseEntity>(sql, new { Email = email });
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<List<EntrepriseDataModel>> ReadAllEntreprisesAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Entreprise]";
            var entities = await connection.QueryAsync<EntrepriseEntity>(sql);
            return entities.Select(MapToModel).ToList();
        }

        private static EntrepriseDataModel MapToModel(EntrepriseEntity entity)
        {
            return new EntrepriseDataModel
            {
                Id = entity.Id,
                RaisonSocial = entity.RaisonSocial,
                StatutJuridique = entity.StatutJuridique,
                Taille = entity.Taille,
                NombreEmployes = entity.NombreEmployes,
                Adresse = entity.Adresse,
                AnneeCreation = entity.AnneeCreation,
                NifStat = entity.NifStat,
                Email = entity.Email
            };
        }
    }
}
