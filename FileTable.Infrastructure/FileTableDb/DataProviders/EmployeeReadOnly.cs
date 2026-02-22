using Dapper;
using Employee.Module.Data.Models;
using Employee.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EmployeeReadOnly : IEmployeeReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public EmployeeReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmployeeDataModel?> GetEmployeeByIdAsync(int id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Employee] WHERE EmployeeID = @Id";
            var entity = await connection.QueryFirstOrDefaultAsync<EmployeeEntity>(sql, new { Id = id });
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<List<EmployeeDataModel>> GetEmployeesByEntrepriseIdAsync(long entrepriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Employee] WHERE EntrepriseID = @EntrepriseID";
            var entities = await connection.QueryAsync<EmployeeEntity>(sql, new { EntrepriseID = entrepriseId });
            return entities.Select(MapToModel).ToList();
        }

        private static EmployeeDataModel MapToModel(EmployeeEntity entity)
        {
            return new EmployeeDataModel
            {
                EmployeeID = entity.EmployeeID,
                Age = entity.Age,
                DateFinContrat = entity.DateFinContrat,
                Email = entity.Email,
                EntrepriseID = entity.EntrepriseID,
                Fonctions = entity.Fonctions,
                IsActive = entity.IsActive,
                Nom = entity.Nom,
                NombreMoisPoste = entity.NombreMoisPoste,
                NomPoste = entity.NomPoste,
                NumeroMatricule = entity.NumeroMatricule,
                Prenom = entity.Prenom,
                Sexe = entity.Sexe,
                StatutEmploye = entity.StatutEmploye,
                VouloirSouscrire = entity.VouloirSouscrire
            };
        }
    }
}
