using Dapper;
using FileTable.Infrastructure.FileTableDb.Entities;
using PaySlip.Module.Data.Models;
using PaySlip.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class PaySlipSecondEntryReadOnly : IPaySlipSecondEntryReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public PaySlipSecondEntryReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaySlipSecondEntryDataModel?> ReadByEmployeeAndPeriodAsync(long employeeId, int periodId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"SELECT * FROM [documentdb].[dbo].[PaySlipSecondEntry]
                        WHERE EmployeeID = @employeeId AND PeriodID = @periodId";
            var entity = await connection.QueryFirstOrDefaultAsync<PaySlipSecondEntryEntity>(sql, new { employeeId, periodId });
            return entity != null ? MapToModel(entity) : null;
        }

        private static PaySlipSecondEntryDataModel MapToModel(PaySlipSecondEntryEntity e)
        {
            return new PaySlipSecondEntryDataModel
            {
                Bonus = e.Bonus,
                CreatedDate = e.CreatedDate,
                EmployeeID = e.EmployeeID,
                IndemniteLogement = e.IndemniteLogement,
                IndemniteTransport = e.IndemniteTransport,
                OvertimeHours = e.OvertimeHours,
                PeriodID = e.PeriodID,
                PrimeScolarite = e.PrimeScolarite,
                SecondEntryID = e.SecondEntryID,
                TreiziemeMois = e.TreiziemeMois
            };
        }
    }
}
