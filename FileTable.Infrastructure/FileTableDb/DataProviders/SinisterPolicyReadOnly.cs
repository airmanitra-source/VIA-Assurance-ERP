using Dapper;
using Sinister.Module.Data.Models;
using Sinister.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class SinisterPolicyReadOnly : ISinisterPolicyReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public SinisterPolicyReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<SinisterPolicyDataModel>> ReadPoliciesByEntrepriseIdAsync(long entrepriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Sinister] WHERE EntrepriseId = @entrepriseId";
            return await connection.QueryAsync<SinisterPolicyDataModel>(sql, new { entrepriseId });
        }

        public async Task<SinisterPolicyDataModel?> ReadPolicyByIdAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Sinister] WHERE Id = @id";
            return await connection.QueryFirstOrDefaultAsync<SinisterPolicyDataModel>(sql, new { id });
        }

        public async Task<SinisterPolicyDataModel?> ReadPolicyByPolicyNumberAsync(string policyNumber)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[Sinister] WHERE PolicyNumber = @policyNumber";
            return await connection.QueryFirstOrDefaultAsync<SinisterPolicyDataModel>(sql, new { policyNumber });
        }
    }
}
