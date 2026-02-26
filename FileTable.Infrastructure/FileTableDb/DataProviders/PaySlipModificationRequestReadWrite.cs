using Dapper;
using Employee.Module.Data.Models;
using Employee.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class PaySlipModificationRequestReadWrite : IPaySlipModificationRequestReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public PaySlipModificationRequestReadWrite(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateRequestAsync(PaySlipModificationRequestDataModel request)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[PaySlipModificationRequest]
                (EmployeeID, PeriodID, Bonus, PrimeScolarite, TreiziemeMois, IndemniteTransport, IndemniteLogement, OvertimeHours, Comments, Status)
                VALUES
                (@EmployeeID, @PeriodID, @Bonus, @PrimeScolarite, @TreiziemeMois, @IndemniteTransport, @IndemniteLogement, @OvertimeHours, @Comments, @Status);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, request);
        }

        public async Task UpdateRequestStatusAsync(int requestId, string status)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[PaySlipModificationRequest]
                SET Status = @status, ReviewedDate = GETDATE()
                WHERE RequestID = @requestId";
            await connection.ExecuteAsync(sql, new { requestId, status });
        }
    }
}
