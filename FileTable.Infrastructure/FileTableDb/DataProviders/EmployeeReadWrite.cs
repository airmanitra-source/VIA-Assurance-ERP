using Dapper;
using Employee.Module.Data.Models;
using Employee.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EmployeeReadWrite : EmployeeReadOnly, IEmployeeReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public EmployeeReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> CreateEmployeeAsync(EmployeeDataModel employee)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[Employee] (Nom, Prenom, Age, Sexe, NomPoste, Fonctions, NombreMoisPoste, StatutEmploye, EntrepriseID, IsActive, NumeroMatricule, DateFinContrat, Email, VouloirSouscrire)
                VALUES (@Nom, @Prenom, @Age, @Sexe, @NomPoste, @Fonctions, @NombreMoisPoste, @StatutEmploye, @EntrepriseID, @IsActive, @NumeroMatricule, @DateFinContrat, @Email, @VouloirSouscrire);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.ExecuteScalarAsync<long>(sql, employee);
        }

        public async Task<IEnumerable<EmployeeDataModel>> ReadEmployeesByEnterpriseAsync(long enterpriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Employee] WHERE EntrepriseID = @enterpriseId";
            return await connection.QueryAsync<EmployeeDataModel>(sql, new { enterpriseId });
        }

        public async Task UpdateEmployeeAsync(EmployeeDataModel employee)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[Employee]
                SET Nom = @Nom,
                    Prenom = @Prenom,
                    Age = @Age,
                    Sexe = @Sexe,
                    NomPoste = @NomPoste,
                    Fonctions = @Fonctions,
                    NombreMoisPoste = @NombreMoisPoste,
                    StatutEmploye = @StatutEmploye,
                    IsActive = @IsActive,
                    NumeroMatricule = @NumeroMatricule,
                    DateFinContrat = @DateFinContrat,
                    Email = @Email,
                    VouloirSouscrire = @VouloirSouscrire
                WHERE EmployeeID = @EmployeeID";

            await connection.ExecuteAsync(sql, employee);
        }

        public async Task UpdateEmployeeActiveStatusAsync(long employeeId, bool isActive, DateTime? dateFinContrat = null)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "UPDATE [documentdb].[dbo].[Employee] SET IsActive = @isActive, DateFinContrat = @dateFinContrat WHERE EmployeeID = @employeeId";
            await connection.ExecuteAsync(sql, new { employeeId, isActive, dateFinContrat });
        }
    }
}
