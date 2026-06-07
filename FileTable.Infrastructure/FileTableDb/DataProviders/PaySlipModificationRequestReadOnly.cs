using Dapper;
using FileTable.Infrastructure.FileTableDb.Entities;
using PaySlip.Module.Data.Models;
using PaySlip.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class PaySlipModificationRequestReadOnly : IPaySlipModificationRequestReadOnlyDataProvider
    {
        private readonly FileTableDbContext _dbContext;

        public PaySlipModificationRequestReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaySlipModificationRequestDataModel?> ReadPendingByEmployeeAndPeriodAsync(long employeeId, int periodId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT TOP 1 *
                FROM [documentdb].[dbo].[PaySlipModificationRequest]
                WHERE EmployeeID = @employeeId AND PeriodID = @periodId AND Status = 'Pending'
                ORDER BY CreatedDate DESC";
            var entity = await connection.QueryFirstOrDefaultAsync<PaySlipModificationRequestEntity>(sql, new { employeeId, periodId });
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<List<PaySlipModificationRequestDataModel>> ReadPendingByPeriodAsync(int periodId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT *
                FROM [documentdb].[dbo].[PaySlipModificationRequest]
                WHERE PeriodID = @periodId AND Status = 'Pending'
                ORDER BY CreatedDate DESC";
            var entities = await connection.QueryAsync<PaySlipModificationRequestEntity>(sql, new { periodId });
            return entities.Select(MapToModel).ToList();
        }

        private static PaySlipModificationRequestDataModel MapToModel(PaySlipModificationRequestEntity e)
        {
            return new PaySlipModificationRequestDataModel
            {
                Bonus = e.Bonus,
                Comments = e.Comments,
                CreatedDate = e.CreatedDate,
                EmployeeID = e.EmployeeID,
                IndemniteLogement = e.IndemniteLogement,
                IndemniteTransport = e.IndemniteTransport,
                OvertimeHours = e.OvertimeHours,
                PeriodID = e.PeriodID,
                PrimeScolarite = e.PrimeScolarite,
                RequestID = e.RequestID,
                ReviewedDate = e.ReviewedDate,
                Status = e.Status,
                TreiziemeMois = e.TreiziemeMois
            };
        }
    }
}

