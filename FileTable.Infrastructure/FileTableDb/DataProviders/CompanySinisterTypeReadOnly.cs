using Dapper;
using Company.Sinister.Module.Data.Models;
using Company.Sinister.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class CompanySinisterTypeReadOnly : ICompanySinisterTypeReadonlyDataProvider
    {
        protected readonly FileTableDbContext _dbContext;

        public CompanySinisterTypeReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CompanySinisterTypeDataModel>> ReadAllAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var entities = await connection.QueryAsync<EntrepriseSinisterTypeEntity>(
                "SELECT * FROM [documentdb].[dbo].[EntrepriseSinisterType] ORDER BY CreatedDate");
            return entities.Select(MapToDataModel).ToList();
        }

        public async Task<CompanySinisterTypeDataModel?> ReadByIdAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<EntrepriseSinisterTypeEntity>(
                "SELECT * FROM [documentdb].[dbo].[EntrepriseSinisterType] WHERE Id = @Id", new { Id = id });
            return entity == null ? null : MapToDataModel(entity);
        }

        public async Task<List<CompanySinisterTypeDataModel>> ReadBySinisterIdAsync(long sinisterId)
        {
            using var connection = _dbContext.CreateConnection();
            var entities = await connection.QueryAsync<EntrepriseSinisterTypeEntity>(
                "SELECT * FROM [documentdb].[dbo].[EntrepriseSinisterType] WHERE EntrepriseSinisterId = @SinisterId",
                new { SinisterId = sinisterId });
            return entities.Select(MapToDataModel).ToList();
        }

        public async Task<List<SinisterTypeDataModel>> ReadSinisterTypesBySinisterIdAsync(long sinisterId)
        {
            using var connection = _dbContext.CreateConnection();
            var entities = await connection.QueryAsync<SinisterTypeEntity>(
                @"SELECT st.* 
                  FROM [documentdb].[dbo].[SinisterType] st
                  INNER JOIN [documentdb].[dbo].[EntrepriseSinisterType] est ON st.Id = est.SinisterTypeId
                  WHERE est.EntrepriseSinisterId = @SinisterId
                  ORDER BY st.TypeName",
                new { SinisterId = sinisterId });
            return entities.Select(e => new SinisterTypeDataModel
            {
                Id = e.Id,
                TypeName = e.TypeName
            }).ToList();
        }

        private static CompanySinisterTypeDataModel MapToDataModel(EntrepriseSinisterTypeEntity entity)
        {
            return new CompanySinisterTypeDataModel
            {
                CreatedDate = entity.CreatedDate,
                EntrepriseSinisterId = entity.EntrepriseSinisterId,
                Id = entity.Id,
                SinisterTypeId = entity.SinisterTypeId
            };
        }
    }
}

