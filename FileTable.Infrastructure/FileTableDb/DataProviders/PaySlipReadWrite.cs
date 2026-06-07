using Dapper;
using EmployeePayroll.Module.Data.Models;
using PaySlip.Module.Data.Models;
using PaySlip.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class PaySlipReadWrite : IPaySlipReadWriteDataProvider
    {
        private readonly FileTableDbContext _dbContext;

        public PaySlipReadWrite(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdatePaySlipAsync(EmployeePayrollDataModel payroll, List<PaySlipLineDataModel> lines)
        {
            using var connection = _dbContext.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var updatePayrollSql = @"
                    UPDATE [documentdb].[dbo].[EmployeePayroll]
                    SET BaseSalary = @BaseSalary, Bonus = @Bonus, Deductions = @Deductions,
                        PaymentDate = @PaymentDate, PaymentMethod = @PaymentMethod, Notes = @Notes,
                        ModifiedDate = GETDATE()
                    WHERE PayrollID = @PayrollID";

                await connection.ExecuteAsync(updatePayrollSql, payroll, transaction);

                var deleteLinesSql = "DELETE FROM [documentdb].[dbo].[PaySlipLine] WHERE PayrollID = @payrollId";
                await connection.ExecuteAsync(deleteLinesSql, new { payrollId = payroll.PayrollID }, transaction);

                var insertLinesSql = @"
                    INSERT INTO [documentdb].[dbo].[PaySlipLine]
                    (PayrollID, PeriodID, EmployeeID, Rubrique, Libelle, LineType, Nombre, Base, Taux, GainAmount, EmployeeDeduction, EmployerContribution, SortOrder)
                    VALUES
                    (@PayrollID, @PeriodID, @EmployeeID, @Rubrique, @Libelle, @LineType, @Nombre, @Base, @Taux, @GainAmount, @EmployeeDeduction, @EmployerContribution, @SortOrder)";

                await connection.ExecuteAsync(insertLinesSql, lines, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

