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

        public async Task<EmployeeDataModel?> ReadEmployeeByEmailAsync(string email)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Employee] WHERE Email = @email";
            var entity = await connection.QueryFirstOrDefaultAsync<EmployeeEntity>(sql, new { email });
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<EmployeeDataModel?> ReadEmployeeByIdAsync(int id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Employee] WHERE EmployeeID = @Id";
            var entity = await connection.QueryFirstOrDefaultAsync<EmployeeEntity>(sql, new { Id = id });
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<List<EmployeeDataModel>> ReadEmployeesByEntrepriseIdAsync(long entrepriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Employee] WHERE EntrepriseID = @EntrepriseID";
            var entities = await connection.QueryAsync<EmployeeEntity>(sql, new { EntrepriseID = entrepriseId });
            return entities.Select(MapToModel).ToList();
        }

        public async Task<List<EmployeeDataModel>> ReadByEnterpriseIdAsync(long enterpriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Employee] WHERE EntrepriseID = @EntrepriseID";
            var entities = await connection.QueryAsync<EmployeeEntity>(sql, new { EntrepriseID = enterpriseId });
            return entities.Select(MapToModel).ToList();
        }

        public async Task<List<EmployeeDataModel>> ReadActiveByEnterpriseIdAsync(long enterpriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Employee] WHERE EntrepriseID = @EntrepriseID AND IsActive = 1";
            var entities = await connection.QueryAsync<EmployeeEntity>(sql, new { EntrepriseID = enterpriseId });
            return entities.Select(MapToModel).ToList();
        }

        public async Task<List<EmployeeDataModel>> ReadEmployeesWithoutPaySlipForPeriodAsync(long enterpriseId, int periodId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT e.*
                FROM [documentdb].[dbo].[Employee] e
                WHERE e.EntrepriseID = @EnterpriseId
                  AND e.IsActive = 1
                  AND NOT EXISTS (
                      SELECT 1
                      FROM [documentdb].[dbo].[PaySlipLine] psl
                      WHERE psl.EmployeeID = e.EmployeeID
                        AND psl.PeriodID = @PeriodId
                  )";
            var entities = await connection.QueryAsync<EmployeeEntity>(sql, new { EnterpriseId = enterpriseId, PeriodId = periodId });
            return entities.Select(MapToModel).ToList();
        }

        private static EmployeeDataModel MapToModel(EmployeeEntity entity)
        {
            return new EmployeeDataModel
            {
                EmployeeID = entity.EmployeeID,
                Age = entity.Age,
                BankAccountNumber = entity.BankAccountNumber,
                Classification = entity.Classification,
                DateEmbauche = entity.DateEmbauche,
                DateFinContrat = entity.DateFinContrat,
                Dependents = entity.NombreEnfants,
                Email = entity.Email,
                EntrepriseID = entity.EntrepriseID,
                Fonctions = entity.Fonctions,
                IsActive = entity.IsActive,
                Nom = entity.Nom,
                NombreMoisPoste = entity.NombreMoisPoste,
                NomPoste = entity.NomPoste,
                NumeroCnaps = entity.NumeroCnaps,
                NumeroMatricule = entity.NumeroMatricule,
                Prenom = entity.Prenom,
                Salaire = entity.Salaire,
                Sexe = entity.Sexe,
                StatutEmploye = entity.StatutEmploye,
                VouloirSouscrire = entity.VouloirSouscrire
            };
        }
    }
}
