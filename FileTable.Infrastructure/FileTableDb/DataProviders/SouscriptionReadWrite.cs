using Dapper;
using Subscription.Module.Data.Models;
using Subscription.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class SouscriptionReadWrite : SouscriptionReadOnly, ISouscriptionReadWriteDataProvider
    {
        private readonly FileTableDbContext _dbContext;

        public SouscriptionReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> CreateSubscriptionAsync(SouscriptionDataModel subscription)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[Souscription] (EmployeeId, EntrepriseId, MoisDeCotisation, AnneeCotisation, MontantCotisation)
                VALUES (@EmployeeId, @EntrepriseId, @MoisDeCotisation, @AnneeCotisation, @MontantCotisation);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            return await connection.ExecuteScalarAsync<long>(sql, subscription);
        }

        public async Task UpdateSubscriptionAsync(SouscriptionDataModel subscription)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[Souscription]
                SET MoisDeCotisation = @MoisDeCotisation,
                    AnneeCotisation = @AnneeCotisation,
                    MontantCotisation = @MontantCotisation
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, subscription);
        }

        public async Task DeleteSubscriptionAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "DELETE FROM [documentdb].[dbo].[Souscription] WHERE Id = @id";
            await connection.ExecuteAsync(sql, new { id });
        }
    }
}

