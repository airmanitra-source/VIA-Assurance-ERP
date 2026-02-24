using Dapper;
using Company.Sinister.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class CompanySinisterTypeReadWrite : CompanySinisterTypeReadOnly, ICompanySinisterTypeReadWrite
    {
        public CompanySinisterTypeReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
        }

        public async Task CreateSinisterTypeAsync(long sinisterId, long sinisterTypeId)
        {
            using var connection = _dbContext.CreateConnection();
            await connection.ExecuteAsync(
                @"INSERT INTO [documentdb].[dbo].[EntrepriseSinisterType] 
                  ([EntrepriseSinisterId], [SinisterTypeId], [CreatedDate])
                  VALUES (@SinisterId, @SinisterTypeId, @CreatedDate)",
                new { SinisterId = sinisterId, SinisterTypeId = sinisterTypeId, CreatedDate = DateTime.UtcNow });
        }

        public async Task CreateSinisterTypesAsync(long sinisterId, IEnumerable<long> sinisterTypeIds)
        {
            using var connection = _dbContext.CreateConnection();
            foreach (var sinisterTypeId in sinisterTypeIds)
            {
                await connection.ExecuteAsync(
                    @"INSERT INTO [documentdb].[dbo].[EntrepriseSinisterType] 
                      ([EntrepriseSinisterId], [SinisterTypeId], [CreatedDate])
                      VALUES (@SinisterId, @SinisterTypeId, @CreatedDate)",
                    new { SinisterId = sinisterId, SinisterTypeId = sinisterTypeId, CreatedDate = DateTime.UtcNow });
            }
        }

        public async Task DeleteSinisterTypesAsync(long sinisterId)
        {
            using var connection = _dbContext.CreateConnection();
            await connection.ExecuteAsync(
                "DELETE FROM [documentdb].[dbo].[EntrepriseSinisterType] WHERE EntrepriseSinisterId = @SinisterId",
                new { SinisterId = sinisterId });
        }
    }
}
