using Dapper;
using Employee.Module.Data.Models;
using Employee.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class PaySlipLineReadWrite : IPaySlipLineReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public PaySlipLineReadWrite(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateLinesAsync(List<PaySlipLineDataModel> lines)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[PaySlipLine] 
                (PayrollID, PeriodID, EmployeeID, Rubrique, Libelle, LineType, Nombre, Base, Taux, GainAmount, EmployeeDeduction, EmployerContribution, SortOrder)
                VALUES 
                (@PayrollID, @PeriodID, @EmployeeID, @Rubrique, @Libelle, @LineType, @Nombre, @Base, @Taux, @GainAmount, @EmployeeDeduction, @EmployerContribution, @SortOrder)";
            await connection.ExecuteAsync(sql, lines);
        }

        public async Task DeleteByPayrollIdAsync(int payrollId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "DELETE FROM [documentdb].[dbo].[PaySlipLine] WHERE PayrollID = @payrollId";
            await connection.ExecuteAsync(sql, new { payrollId });
        }
    }
}
