using Dapper;
using Company.Fleet.Module.Data.Models;
using Company.Fleet.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EntrepriseFleetReadOnly : IEntrepriseFleetReadOnlyDataProvider
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
                EntrepriseId = entity.EntrepriseId,
                FranchiseAmount = entity.FranchiseAmount,
                FranchisePercentage = entity.FranchisePercentage,
                FranchiseType = entity.FranchiseType,
                FiscalPower = entity.FiscalPower,
                Id = entity.Id,
                InsuranceEndDate = entity.InsuranceEndDate,
                InsuranceStartDate = entity.InsuranceStartDate,
                IsInsured = entity.IsInsured,
                IsWorking = entity.IsWorking,
                LicensePlate = entity.LicensePlate,
                Make = entity.Make,
                Mileage = entity.Mileage,
                Model = entity.Model,
                Power = entity.Power,
                Type = entity.Type,
                VIN = entity.VIN,
                WantsInsurance = entity.WantsInsurance,
                Year = entity.Year
            };
        }
    }
}

