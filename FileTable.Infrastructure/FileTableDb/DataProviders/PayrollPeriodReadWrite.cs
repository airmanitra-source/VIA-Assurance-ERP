using Dapper;
using EmployeePayroll.Module.Data.Models;
using EmployeePayroll.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class PayrollPeriodReadWrite : IPayrollPeriodReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public PayrollPeriodReadWrite(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreatePeriodAsync(PayrollPeriodDataModel period)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[PayrollPeriod] (EntrepriseID, PeriodStart, PeriodEnd, PaymentDate, Status)
                VALUES (@EntrepriseID, @PeriodStart, @PeriodEnd, @PaymentDate, @Status);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, period);
        }

        public async Task UpdatePeriodStatusAsync(int periodId, string status)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"UPDATE [documentdb].[dbo].[PayrollPeriod] 
                        SET Status = @status, ModifiedDate = GETDATE() 
                        WHERE PeriodID = @periodId";
            await connection.ExecuteAsync(sql, new { periodId, status });
        }
    }
}
