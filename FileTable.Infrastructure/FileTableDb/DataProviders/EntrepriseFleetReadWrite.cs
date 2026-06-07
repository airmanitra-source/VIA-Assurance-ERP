using Dapper;
using Company.Fleet.Module.Data.Models;
using Company.Fleet.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EntrepriseFleetReadWrite : EntrepriseFleetReadOnly, IEntrepriseFleetReadWriteDataProvider
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
                INSERT INTO [documentdb].[dbo].[EntrepriseFleet] 
                (EntrepriseId, FranchiseAmount, FranchisePercentage, FranchiseType, FiscalPower, InsuranceEndDate, InsuranceStartDate, IsInsured, IsWorking, LicensePlate, Make, Mileage, Model, PolicyNumber, Power, Type, VIN, WantsInsurance, Year)
                VALUES 
                (@EntrepriseId, @FranchiseAmount, @FranchisePercentage, @FranchiseType, @FiscalPower, @InsuranceEndDate, @InsuranceStartDate, @IsInsured, @IsWorking, @LicensePlate, @Make, @Mileage, @Model, @PolicyNumber, @Power, @Type, @VIN, @WantsInsurance, @Year);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            return await connection.ExecuteScalarAsync<long>(sql, fleetItem);
        }

        public async Task UpdateFleetItemAsync(EntrepriseFleetDataModel fleetItem)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[EntrepriseFleet]
                SET EntrepriseId = @EntrepriseId,
                    FranchiseAmount = @FranchiseAmount,
                    FranchisePercentage = @FranchisePercentage,
                    FranchiseType = @FranchiseType,
                    FiscalPower = @FiscalPower,
                    InsuranceEndDate = @InsuranceEndDate,
                    InsuranceStartDate = @InsuranceStartDate,
                    IsInsured = @IsInsured,
                    IsWorking = @IsWorking,
                    LicensePlate = @LicensePlate,
                    Make = @Make,
                    Mileage = @Mileage,
                    Model = @Model,
                    PolicyNumber = @PolicyNumber,
                    Power = @Power,
                    Type = @Type,
                    VIN = @VIN,
                    WantsInsurance = @WantsInsurance,
                    Year = @Year
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

