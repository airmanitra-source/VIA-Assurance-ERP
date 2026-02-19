using Dapper;
using Company.Transportation.Module.Data.Models;
using Company.Transportation.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EntrepriseMerchandiseTransportationReadOnly : IEntrepriseMerchandiseTransportationReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public EntrepriseMerchandiseTransportationReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EntrepriseMerchandiseTransportationDataModel?> GetTransportationByIdAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[EntrepriseMerchandiseTransportation] WHERE Id = @Id";
            var entity = await connection.QueryFirstOrDefaultAsync<EntrepriseMerchandiseTransportationEntity>(sql, new { Id = id });
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<IEnumerable<EntrepriseMerchandiseTransportationDataModel>> GetTransportationsByEntrepriseIdAsync(long entrepriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[EntrepriseMerchandiseTransportation] WHERE EntrepriseId = @EntrepriseId";
            var entities = await connection.QueryAsync<EntrepriseMerchandiseTransportationEntity>(sql, new { EntrepriseId = entrepriseId });
            return entities.Select(MapToModel);
        }

        protected EntrepriseMerchandiseTransportationDataModel MapToModel(EntrepriseMerchandiseTransportationEntity entity)
        {
            return new EntrepriseMerchandiseTransportationDataModel
            {
                Id = entity.Id,
                EntrepriseId = entity.EntrepriseId,
                Description = entity.Description,
                Value = entity.Value,
                DepartureDate = entity.DepartureDate,
                ArrivalDate = entity.ArrivalDate,
                Origin = entity.Origin,
                Destination = entity.Destination,
                Frequency = entity.Frequency,
                WantsInsurance = entity.WantsInsurance,
                IsInsured = entity.IsInsured
            };
        }
    }
}
