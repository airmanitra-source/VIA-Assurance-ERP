using Dapper;
using Company.Fleet.Module.Data.Models;
using Company.Fleet.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EntrepriseFleetReadWrite : EntrepriseFleetReadOnly, IEntrepriseFleetReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public EntrepriseFleetReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> AddFleetItemAsync(EntrepriseFleetDataModel fleetItem)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[EntrepriseFleet] (EntrepriseId, Type, Year, IsWorking, Mileage, Make, Model, WantsInsurance, IsInsured, PolicyNumber)
                VALUES (@EntrepriseId, @Type, @Year, @IsWorking, @Mileage, @Make, @Model, @WantsInsurance, @IsInsured, @PolicyNumber);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            return await connection.ExecuteScalarAsync<long>(sql, fleetItem);
        }

        public async Task UpdateFleetItemAsync(EntrepriseFleetDataModel fleetItem)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[EntrepriseFleet]
                SET Type = @Type,
                    Year = @Year,
                    IsWorking = @IsWorking,
                    Mileage = @Mileage,
                    Make = @Make,
                    Model = @Model,
                    WantsInsurance = @WantsInsurance,
                    IsInsured = @IsInsured,
                    PolicyNumber = @PolicyNumber
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, fleetItem);
        }

        public async Task DeleteFleetItemAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "DELETE FROM [documentdb].[dbo].[EntrepriseFleet] WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
