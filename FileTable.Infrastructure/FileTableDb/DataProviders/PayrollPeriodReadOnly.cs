using Dapper;
using EmployeePayroll.Module.Data.Models;
using EmployeePayroll.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class PayrollPeriodReadOnly : IPayrollPeriodReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public PayrollPeriodReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PayrollPeriodDataModel>> ReadByEnterpriseIdAsync(long enterpriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"SELECT * FROM [documentdb].[dbo].[PayrollPeriod] 
                        WHERE EntrepriseID = @enterpriseId 
                        ORDER BY PeriodStart DESC";
            var entities = await connection.QueryAsync<PayrollPeriodEntity>(sql, new { enterpriseId });
            return entities.Select(MapToModel).ToList();
        }

        public async Task<PayrollPeriodDataModel?> ReadByIdAsync(int periodId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[PayrollPeriod] WHERE PeriodID = @periodId";
            var entity = await connection.QueryFirstOrDefaultAsync<PayrollPeriodEntity>(sql, new { periodId });
            return entity != null ? MapToModel(entity) : null;
        }

        private static PayrollPeriodDataModel MapToModel(PayrollPeriodEntity e)
        {
            return new PayrollPeriodDataModel
            {
                EntrepriseID = e.EntrepriseID,
                PaymentDate = e.PaymentDate,
                PeriodEnd = e.PeriodEnd,
                PeriodID = e.PeriodID,
                PeriodStart = e.PeriodStart,
                Status = e.Status
            };
        }
    }
}
