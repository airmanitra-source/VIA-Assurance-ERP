using Dapper;
using PaySlip.Module.Data.Models;
using PaySlip.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class PaySlipSecondEntryReadWrite : IPaySlipSecondEntryReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public PaySlipSecondEntryReadWrite(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateSecondEntryAsync(PaySlipSecondEntryDataModel entry)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                DELETE FROM [documentdb].[dbo].[PaySlipSecondEntry]
                WHERE EmployeeID = @EmployeeID AND PeriodID = @PeriodID;

                INSERT INTO [documentdb].[dbo].[PaySlipSecondEntry]
                (EmployeeID, PeriodID, Bonus, PrimeScolarite, TreiziemeMois, IndemniteTransport, IndemniteLogement, OvertimeHours)
                VALUES
                (@EmployeeID, @PeriodID, @Bonus, @PrimeScolarite, @TreiziemeMois, @IndemniteTransport, @IndemniteLogement, @OvertimeHours);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, entry);
        }

        public async Task DeleteByEmployeeAndPeriodAsync(long employeeId, int periodId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"DELETE FROM [documentdb].[dbo].[PaySlipSecondEntry]
                        WHERE EmployeeID = @employeeId AND PeriodID = @periodId";
            await connection.ExecuteAsync(sql, new { employeeId, periodId });
        }
    }
}
