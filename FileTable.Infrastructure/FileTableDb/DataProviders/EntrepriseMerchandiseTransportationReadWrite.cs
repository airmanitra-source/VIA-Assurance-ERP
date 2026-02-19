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
                INSERT INTO [documentdb].[dbo].[EntrepriseMerchandiseTransportation] (EntrepriseId, Description, Value, DepartureDate, ArrivalDate, Origin, Destination, Frequency, WantsInsurance, IsInsured, PolicyNumber)
                VALUES (@EntrepriseId, @Description, @Value, @DepartureDate, @ArrivalDate, @Origin, @Destination, @Frequency, @WantsInsurance, @IsInsured, @PolicyNumber);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            return await connection.ExecuteScalarAsync<long>(sql, transportation);
        }

        public async Task UpdateTransportationAsync(EntrepriseMerchandiseTransportationDataModel transportation)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[EntrepriseMerchandiseTransportation]
                SET Description = @Description,
                    Value = @Value,
                    DepartureDate = @DepartureDate,
                    ArrivalDate = @ArrivalDate,
                    Origin = @Origin,
                    Destination = @Destination,
                    Frequency = @Frequency,
                    WantsInsurance = @WantsInsurance,
                    IsInsured = @IsInsured,
                    PolicyNumber = @PolicyNumber
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
