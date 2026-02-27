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

        public async Task<long> CreateEmployeeAsync(EmployeeDataModel employee, int? projectId = null)
        {
            using var connection = _dbContext.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            var insertEmployeeSql = @"
                INSERT INTO [documentdb].[dbo].[Employee] (Nom, Prenom, Age, Sexe, Salaire, NomPoste, Fonctions, NombreEnfants, NombreMoisPoste, StatutEmploye, EntrepriseID, IsActive, NumeroMatricule, DateEmbauche, DateFinContrat, Email, VouloirSouscrire)
                VALUES (@Nom, @Prenom, @Age, @Sexe, @Salaire, @NomPoste, @Fonctions, @Dependents, @NombreMoisPoste, @StatutEmploye, @EntrepriseID, @IsActive, @NumeroMatricule, @DateEmbauche, @DateFinContrat, @Email, @VouloirSouscrire);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            var employeeId = await connection.ExecuteScalarAsync<long>(insertEmployeeSql, employee, transaction);

            int? resolvedProjectId = projectId;
            if (!resolvedProjectId.HasValue)
            {
                var onBoardingProjectIdSql = "SELECT ProjectID FROM [documentdb].[dbo].[Project] WHERE ProjectName = 'On Boarding'";
                resolvedProjectId = await connection.ExecuteScalarAsync<int?>(onBoardingProjectIdSql, transaction: transaction);
            }

            if (resolvedProjectId.HasValue)
            {
                var insertProjectSql = @"
                    INSERT INTO [documentdb].[dbo].[EmployeeProject] (EmployeeID, ProjectID, Role, AssignedDate, IsActive, CreatedDate)
                    VALUES (@employeeId, @projectId, 'Employee', GETDATE(), 1, GETDATE());";

                await connection.ExecuteAsync(insertProjectSql, new { employeeId, projectId = resolvedProjectId.Value }, transaction);
            }

            transaction.Commit();
            return employeeId;
        }

        public async Task<IEnumerable<EmployeeDataModel>> ReadEmployeesByEnterpriseAsync(long enterpriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT *, NombreEnfants AS Dependents FROM [documentdb].[dbo].[Employee] WHERE EntrepriseID = @enterpriseId";
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
                    Salaire = @Salaire,
                    NomPoste = @NomPoste,
                    Fonctions = @Fonctions,
                    NombreEnfants = @Dependents,
                    NombreMoisPoste = @NombreMoisPoste,
                    StatutEmploye = @StatutEmploye,
                    IsActive = @IsActive,
                    NumeroMatricule = @NumeroMatricule,
                    DateEmbauche = @DateEmbauche,
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
