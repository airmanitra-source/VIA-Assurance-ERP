using Dapper;
using Subscription.Module.Data.Models;
using Subscription.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class SouscriptionReadOnly : ISouscriptionReadOnlyDataProvider
    {
        private readonly FileTableDbContext _dbContext;

        public SouscriptionReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<SouscriptionDataModel>> ReadSubscriptionsByEmployeeIdAsync(int employeeId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Souscription] WHERE EmployeeId = @employeeId";
            return await connection.QueryAsync<SouscriptionDataModel>(sql, new { employeeId });
        }

        public async Task<IEnumerable<SouscriptionDataModel>> ReadSubscriptionsByEnterpriseIdAsync(long enterpriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Souscription] WHERE EntrepriseId = @enterpriseId";
            return await connection.QueryAsync<SouscriptionDataModel>(sql, new { enterpriseId });
        }

        public async Task<SouscriptionDataModel?> ReadSubscriptionByIdAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Souscription] WHERE Id = @id";
            return await connection.QueryFirstOrDefaultAsync<SouscriptionDataModel>(sql, new { id });
        }

        public async Task<int> ReadMaxSubscriptionMonthAsync(int employeeId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT ISNULL(MAX(MoisDeCotisation), 0) FROM [documentdb].[dbo].[Souscription] WHERE EmployeeId = @employeeId";
            return await connection.ExecuteScalarAsync<int>(sql, new { employeeId });
        }

        public async Task<bool> ExistsAsync(int employeeId, int month, int year)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT COUNT(1) FROM [documentdb].[dbo].[Souscription] WHERE EmployeeId = @employeeId AND MoisDeCotisation = @month AND AnneeCotisation = @year";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { employeeId, month, year });
            return count > 0;
        }
    }
}

