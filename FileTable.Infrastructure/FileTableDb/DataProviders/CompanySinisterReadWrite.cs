using Dapper;
using Company.Sinister.Module.Data.Models;
using Company.Sinister.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class CompanySinisterReadWrite : CompanySinisterReadOnly, ICompanySinisterReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public CompanySinisterReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> AddSinisterAsync(CompanySinisterDataModel sinister)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[EntrepriseSinister]
                    (EntrepriseId, SinisterDate, Description, Status, EstimatedAmount, ResolvedAmount,
                     AssetType, EntrepriseFleetId, EntrepriseMerchandiseTransportationId, EntrepriseWarehouseId,
                     SinisterId, CreatedDate, LastModifiedDate)
                VALUES
                    (@EntrepriseId, @SinisterDate, @Description, @Status, @EstimatedAmount, @ResolvedAmount,
                     @AssetType, @EntrepriseFleetId, @EntrepriseMerchandiseTransportationId, @EntrepriseWarehouseId,
                     @SinisterId, @CreatedDate, @LastModifiedDate);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            return await connection.ExecuteScalarAsync<long>(sql, sinister);
        }

        public async Task UpdateSinisterAsync(CompanySinisterDataModel sinister)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[EntrepriseSinister]
                SET Status             = @Status,
                    Description        = @Description,
                    EstimatedAmount    = @EstimatedAmount,
                    ResolvedAmount     = @ResolvedAmount,
                    LastModifiedDate   = @LastModifiedDate
                WHERE Id = @Id";
            await connection.ExecuteAsync(sql, sinister);
        }

        public async Task DeleteSinisterAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            await connection.ExecuteAsync(
                "DELETE FROM [documentdb].[dbo].[EntrepriseSinister] WHERE Id = @Id", new { Id = id });
        }
    }
}
