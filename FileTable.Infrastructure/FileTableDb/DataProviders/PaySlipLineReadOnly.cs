using Dapper;
using FileTable.Infrastructure.FileTableDb.Entities;
using PaySlip.Module.Data.Models;
using PaySlip.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class PaySlipLineReadOnly : IPaySlipLineReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public PaySlipLineReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PaySlipLineDataModel>> ReadByPayrollIdAsync(int payrollId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"SELECT * FROM [documentdb].[dbo].[PaySlipLine] 
                        WHERE PayrollID = @payrollId ORDER BY SortOrder";
            var entities = await connection.QueryAsync<PaySlipLineEntity>(sql, new { payrollId });
            return entities.Select(MapToModel).ToList();
        }

        public async Task<List<PaySlipLineDataModel>> ReadByPeriodAndEmployeeAsync(int periodId, long employeeId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"SELECT * FROM [documentdb].[dbo].[PaySlipLine] 
                        WHERE PeriodID = @periodId AND EmployeeID = @employeeId 
                        ORDER BY SortOrder";
            var entities = await connection.QueryAsync<PaySlipLineEntity>(sql, new { periodId, employeeId });
            return entities.Select(MapToModel).ToList();
        }

        private static PaySlipLineDataModel MapToModel(PaySlipLineEntity e)
        {
            return new PaySlipLineDataModel
            {
                Base = e.Base,
                EmployeeDeduction = e.EmployeeDeduction,
                EmployeeID = e.EmployeeID,
                EmployerContribution = e.EmployerContribution,
                GainAmount = e.GainAmount,
                Libelle = e.Libelle,
                LineID = e.LineID,
                LineType = e.LineType,
                Nombre = e.Nombre,
                PayrollID = e.PayrollID,
                PeriodID = e.PeriodID,
                Rubrique = e.Rubrique,
                SortOrder = e.SortOrder,
                Taux = e.Taux
            };
        }
    }
}
