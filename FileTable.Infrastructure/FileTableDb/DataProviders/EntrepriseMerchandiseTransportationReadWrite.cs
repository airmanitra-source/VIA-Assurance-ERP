using Dapper;
using Company.Transportation.Module.Data.Models;
using Company.Transportation.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EntrepriseMerchandiseTransportationReadWrite : EntrepriseMerchandiseTransportationReadOnly, IEntrepriseMerchandiseTransportationReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public EntrepriseMerchandiseTransportationReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> AddTransportationAsync(EntrepriseMerchandiseTransportationDataModel transportation)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[EntrepriseMerchandiseTransportation] (ArrivalDate, DepartureDate, Description, Destination, EntrepriseId, Frequency, InsuranceEndDate, InsuranceStartDate, IsInsured, Origin, PolicyNumber, Value, WantsInsurance)
                VALUES (@ArrivalDate, @DepartureDate, @Description, @Destination, @EntrepriseId, @Frequency, @InsuranceEndDate, @InsuranceStartDate, @IsInsured, @Origin, @PolicyNumber, @Value, @WantsInsurance);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            return await connection.ExecuteScalarAsync<long>(sql, transportation);
        }

        public async Task UpdateTransportationAsync(EntrepriseMerchandiseTransportationDataModel transportation)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[EntrepriseMerchandiseTransportation]
                SET ArrivalDate = @ArrivalDate,
                    DepartureDate = @DepartureDate,
                    Description = @Description,
                    Destination = @Destination,
                    EntrepriseId = @EntrepriseId,
                    Frequency = @Frequency,
                    InsuranceEndDate = @InsuranceEndDate,
                    InsuranceStartDate = @InsuranceStartDate,
                    IsInsured = @IsInsured,
                    Origin = @Origin,
                    PolicyNumber = @PolicyNumber,
                    Value = @Value,
                    WantsInsurance = @WantsInsurance
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, transportation);
        }

        public async Task DeleteTransportationAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "DELETE FROM [documentdb].[dbo].[EntrepriseMerchandiseTransportation] WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
