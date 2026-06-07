using Dapper;
using Company.Sinister.Module.Data.Models;
using Company.Sinister.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class SinisterTypeReadOnly : ISinisterTypeReadonlyDataProvider
    {
        protected readonly FileTableDbContext _dbContext;

        public SinisterTypeReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SinisterTypeDataModel>> ReadAllAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var entities = await connection.QueryAsync<SinisterTypeEntity>(
                "SELECT * FROM [documentdb].[dbo].[SinisterType] ORDER BY TypeName");
            return entities.Select(MapToDataModel).ToList();
        }

        public async Task<SinisterTypeDataModel?> ReadByIdAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<SinisterTypeEntity>(
                "SELECT * FROM [documentdb].[dbo].[SinisterType] WHERE Id = @Id", new { Id = id });
            return entity == null ? null : MapToDataModel(entity);
        }

        public async Task<SinisterTypeDataModel?> ReadByNameAsync(string typeName)
        {
            using var connection = _dbContext.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<SinisterTypeEntity>(
                "SELECT * FROM [documentdb].[dbo].[SinisterType] WHERE TypeName = @TypeName", new { TypeName = typeName });
            return entity == null ? null : MapToDataModel(entity);
        }

        private static SinisterTypeDataModel MapToDataModel(SinisterTypeEntity entity)
        {
            return new SinisterTypeDataModel
            {
                Id = entity.Id,
                TypeName = entity.TypeName
            };
        }
    }
}

