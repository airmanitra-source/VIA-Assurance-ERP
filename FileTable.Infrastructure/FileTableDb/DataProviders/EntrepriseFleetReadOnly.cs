using Dapper;
using Company.Fleet.Module.Data.Models;
using Company.Fleet.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EntrepriseFleetReadOnly : IEntrepriseFleetReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public EntrepriseFleetReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EntrepriseFleetDataModel?> GetFleetItemByIdAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[EntrepriseFleet] WHERE Id = @Id";
            var entity = await connection.QueryFirstOrDefaultAsync<EntrepriseFleetEntity>(sql, new { Id = id });
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<IEnumerable<EntrepriseFleetDataModel>> GetFleetByEntrepriseIdAsync(long entrepriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[EntrepriseFleet] WHERE EntrepriseId = @EntrepriseId";
            var entities = await connection.QueryAsync<EntrepriseFleetEntity>(sql, new { EntrepriseId = entrepriseId });
            return entities.Select(MapToModel);
        }

        protected EntrepriseFleetDataModel MapToModel(EntrepriseFleetEntity entity)
        {
            return new EntrepriseFleetDataModel
            {
                Id = entity.Id,
                EntrepriseId = entity.EntrepriseId,
                Type = entity.Type,
                Year = entity.Year,
                IsWorking = entity.IsWorking,
                Mileage = entity.Mileage,
                Make = entity.Make,
                Model = entity.Model,
                WantsInsurance = entity.WantsInsurance,
                IsInsured = entity.IsInsured
            };
        }
    }
}
